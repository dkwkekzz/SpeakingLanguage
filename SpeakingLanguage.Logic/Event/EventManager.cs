using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    /// <summary>
    /// 1. serialize관련 함수를 모두 여기로 뺀다. 즉, service는 데이터만 제공해주며, 진입지점과 어떻게 집어넣을지는 eventmanager에서 관리한다.
    /// 2. 집어넣는 작업과 로직수행작업을 절대로 동시에하지 않는다. 
    /// 3. 2번으로 인해서 datalist에 담지 않고, service에 즉시 집어넣는다. 시리얼라이즈도 마찬가지이다.
    /// 4. 시작시에 그래프를 그루핑한다. scc로 구분된 그룹으로 나누어 이를 스레드에게 분배한다. 
    /// 5. 각 그룹에서는 엑션을 시작한다. 최초에 마주한 객체에대하여 self를 수행하고 이후 dfs로 연결된 노드끼리 complex를 수행한다. 
    /// 6. target의 add는 postprocess를 이용하여 처리하도록 한다.
    /// </summary>
    public sealed class EventManager : Library.SingletonLazy<EventManager>
    {
        private Service _logicService;
        private int _currentFrame;
        public int CurrentFrame => _currentFrame;

        //private readonly List<int> _tempList = new List<int>();
        //private readonly Dictionary<int, int> _tempDic = new Dictionary<int, int>();
        //private readonly Queue<int> _tempQueue = new Queue<int>();
        //
        //private DataList<Controller> _controllers;
        //private DataList<Interaction> _interactions;
        //
        //public ref Service Service => ref _logicService;

        public void Install(ref StartInfo info)
        {
            _logicService = new Service(ref info);
            //_controllers = new DataList<Controller>(32);
            //_interactions = new DataList<Interaction>(32, new InteractionComparer());
        }

        public void Uninstall()
        {
            _logicService.Dispose();
        }

        public void Insert(Controller stEvent)
        {
            _controllers.Add(stEvent);
        }

        public void Insert(Interaction stEvent)
        {
            var lhsValue = stEvent.lhs.value;
            var rhsValue = stEvent.rhs.value;
            if (lhsValue == rhsValue)
            {
                Library.Tracer.Error($"could not self interact: {lhsValue.ToString()} to {rhsValue.ToString()}");
                return;
            }

            _interactions.Add(new Interaction
            {
                lhs = lhsValue < rhsValue ? stEvent.lhs : stEvent.rhs,
                rhs = lhsValue < rhsValue ? stEvent.rhs : stEvent.lhs,
            });
        }
        
        public void FrameEnter()
        {
            _logicService.Begin();
            _currentFrame++;
            //_controllers.Swap();
            //_interactions.Swap();
        }
        
        public FrameResult ExecuteFrame()
        {
            Process.Director.Execute(this, ref _logicService);
            Process.Interactor.Execute(this, ref _logicService);
            Process.Factory.Execute(this, ref _logicService);

            return _logicService.End();
        }

        #region INTERNAL
        //internal DataList<Controller>.Enumerator GetControllerEnumerator()
        //{
        //    return _controllers.GetEnumerator();
        //}
        //
        //internal DataList<Interaction>.Enumerator GetInteractionEnumerator()
        //{
        //    return _interactions.GetSortedEnumerator();
        //}
        //
        //internal InteractionGraph GetInteractionGraph()
        //{
        //    _tempList.Clear();
        //    _tempDic.Clear();
        //    _tempQueue.Clear();
        //
        //    var list = _interactions;
        //    var selectedKey = -1;
        //    var index = 0;
        //    var iter = list.GetSortedEnumerator();
        //    while (iter.MoveNext())
        //    {
        //        var key = iter.Current.lhs.value;
        //        if (selectedKey != key)
        //        {
        //            _tempDic.Add(key, index);
        //            _tempList.Add(-1);   // token
        //            _tempList.Add(key);  // key
        //
        //            selectedKey = key;
        //        }
        //
        //        _tempList.Add(iter.Current.rhs.value);
        //        index++;
        //    }
        //
        //    return new InteractionGraph(_tempList, _tempDic, _tempQueue);
        //}
        #endregion
    }
}
