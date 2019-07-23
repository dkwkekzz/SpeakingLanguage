//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Linq;

//namespace SpeakingLanguage.Library
//{
//    public interface IProvider
//    {
//        //TService Get<TService>() where TService : class, IService, new();
//    }

//    public abstract class Provider : IProvider
//    {
//        private TypeMap<IService> _serviceMap = new TypeMap<IService>();
//        private bool isLock = true;

//        protected void _loadServicesAssembly()
//        {
//            Assembly a = Assembly.GetCallingAssembly();
//            var serviceTypes = from type in a.ExportedTypes
//                               where type.IsClass && typeof(IService).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())
//                               //from attr in type.GetCustomAttributes()
//                               //where attr is ServiceAttribute
//                               select type;

//            foreach (var serviceType in serviceTypes)
//            {
//                _regist(serviceType, (IService)System.Activator.CreateInstance(serviceType));
//            }
//        }

//        protected void _regist(IService service)
//        {
//            _regist(typeof(IService), service);
//        }

//        protected void _regist(Type type, IService service)
//        {
//            if (_serviceMap.Contains(type))
//                // 이전에 초기화한 객체가 해당 객체에 접근했다면, 미리 초기화했다.
//                return;

//            try
//            {
//                //service.Initialize(this);
//            }
//            catch
//            {
//            }

//            _serviceMap.Add(type, service);
//        }

//        public virtual void Initialize()
//        {
//            _loadServicesAssembly();
//            isLock = false;
//        }

//        public TService Get<TService>()
//            where TService : class, IService, new()
//        {
//            if (isLock)
//                throw new TypeAccessException("provider instancing...");

//            if (!_serviceMap.Contains(typeof(TService)))
//                _regist(new TService());

//            return _serviceMap.Get<TService>();
//        }

//        public TInterface GetInterface<TInterface>()
//            where TInterface : class
//        {
//            foreach (var pair in _serviceMap)
//            {
//                if (pair.Value is TInterface)
//                    return pair.Value as TInterface;
//            }

//            return null;
//        }
//    }
//}
