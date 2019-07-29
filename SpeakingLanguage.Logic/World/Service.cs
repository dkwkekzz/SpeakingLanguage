using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.World
{
    internal sealed class Service : IDisposable
    {
        sealed class _CellComparer : IComparer<sCell>
        {
            public int Compare(sCell x, sCell y)
            {
                return x.Handle.CompareTo(y.Handle);
            }
        }

        sealed class _CellNodeComparer : IComparer<sCellNode>
        {
            public int Compare(sCellNode x, sCellNode y)
            {
                return x.ObHandle.CompareTo(y.ObHandle);
            }
        }

        private readonly Library.umnMarshal _marshal;
        private readonly Library.umnHeap _worldHeap;
        
        private readonly Library.umnFactory<Library.umnHeap, sCell> _sfCell;
        private readonly Library.umnSplayBT<Library.umnHeap, sCell> _sbtCell;
        private readonly Library.umnFactory<Library.umnHeap, sCellNode> _sfCellNode;
        private readonly Library.umnSplayBT<Library.umnHeap, sCellNode> _sbtCellNode;
        
        public Service(ref StartInfo info)
        {
            _marshal = new Library.umnMarshal();
            
            unsafe
            {
                _worldHeap = new Library.umnHeap(_marshal.Alloc(info.max_byte_world_service));

                fixed (Library.umnHeap* pHeap = &_worldHeap)
                {
                    _sfCell = new Library.umnFactory<Library.umnHeap, sCell>(pHeap, info.default_count_cell);
                    _sbtCell = new Library.umnSplayBT<Library.umnHeap, sCell>(pHeap, info.default_count_cell, new _CellComparer());
                    _sfCellNode = new Library.umnFactory<Library.umnHeap, sCellNode>(pHeap, info.default_count_cellnode);
                    _sbtCellNode = new Library.umnSplayBT<Library.umnHeap, sCellNode>(pHeap, info.default_count_cellnode, new _CellNodeComparer());
                }
            }
        }
        
        public void OnEvent(Terminal terminal)
        {
            _popTransformAndExchangeCell(terminal);

            _pushSimpleInteraction(terminal);

            _pushSelfInteraction(terminal);
        }

        private unsafe void _popTransformAndExchangeCell(Terminal terminal)
        {
            var requester = terminal.GetRequester<Event.Transform>();
            while (!requester.IsEmpty)
            {
                var tr = requester.Pop();
                var key = new sCellNode(tr->handle);

                if (tr->layer == (int)Define.Layer.None)
                {
                    if (!_sbtCellNode.TryGetValue(&key, out sCellNode* node))
                        Library.ThrowHelper.ThrowKeyNotFound($"not exist handle in _sbtCellNode: {tr->handle.ToString()}");

                    var pc = node->Parent;
                    if (null != pc)
                    {
                        pc->Remove(node);
                        if (pc->Tail == null)
                        {
                            _sbtCell.Remove(pc);
                            pc->Release();
                            _sfCell.PutObject(pc);
                        }
                    }

                    _sbtCellNode.Remove(node);
                    node->Release();
                    _sfCellNode.PutObject(node);

                    return;
                }

                if (tr->layer == (int)Define.Layer.World)
                {
                    if (!_sbtCellNode.TryGetValue(&key, out sCellNode* node))
                    {
                        node = _sfCellNode.GetObject();
                        node->Take(tr->handle);

                        _sbtCellNode.Add(node);
                    }

                    var prevCell = node->Parent;
                    var nextCell = _selectCell(tr->x, tr->y);
                    if (prevCell != nextCell)
                    {
                        if (prevCell != null)
                            prevCell->Remove(node);

                        nextCell->Add(node);
                    }
                }
            }
        }

        private unsafe sCell* _selectCell(int x, int y)
        {
            var xi = x >> sCell.BLOCK_BIT;
            var yi = y >> sCell.BLOCK_BIT;

            var handle = (xi << sCell.MAX_BIT) | yi;

            var key = new sCell(handle);
            if (!_sbtCell.TryGetValue(&key, out sCell* c))
            {
                c = _sfCell.GetObject();
                c->Take(_sbtCell, handle);

                _sbtCell.Add(c);
            }

            return c;
        }

        private unsafe void _pushSimpleInteraction(Terminal terminal)
        {
            var poster = terminal.GetPoster<Event.SimpleInteraction>();
            var pairIter = _sbtCellNode.GetEnumerator();
            while (pairIter.MoveNext())
            {
                var node = pairIter.Current;
                var handle = node->ObHandle;

                var parentCell = node->Parent;
                var parentIter = parentCell->GetEnumerator();
                while (parentIter.MoveNext())
                {
                    if (node == parentIter.Current)
                        continue;

                    poster.Push(new Event.SimpleInteraction { lhs = handle, rhs = parentIter.Current->ObHandle });
                }

                if (null != parentCell->RightCell)
                {
                    var rightIter = parentCell->RightCell->GetEnumerator();
                    while (rightIter.MoveNext())
                    {
                        poster.Push(new Event.SimpleInteraction { lhs = handle, rhs = parentIter.Current->ObHandle });
                        poster.Push(new Event.SimpleInteraction { lhs = parentIter.Current->ObHandle, rhs = handle });
                    }
                }

                if (null != parentCell->BottomCell)
                {
                    var bottomIter = parentCell->BottomCell->GetEnumerator();
                    while (bottomIter.MoveNext())
                    {
                        poster.Push(new Event.SimpleInteraction { lhs = handle, rhs = parentIter.Current->ObHandle });
                        poster.Push(new Event.SimpleInteraction { lhs = parentIter.Current->ObHandle, rhs = handle });
                    }
                }

                if (null != parentCell->RightBottomCell)
                {
                    var rightBottomIter = parentCell->RightBottomCell->GetEnumerator();
                    while (rightBottomIter.MoveNext())
                    {
                        poster.Push(new Event.SimpleInteraction { lhs = handle, rhs = parentIter.Current->ObHandle });
                        poster.Push(new Event.SimpleInteraction { lhs = parentIter.Current->ObHandle, rhs = handle });
                    }
                }
            }
        }

        private unsafe void _pushSelfInteraction(Terminal terminal)
        {
            var poster = terminal.GetPoster<Event.SelfInteraction>();
            var pairIter = _sbtCellNode.GetEnumerator();
            while (pairIter.MoveNext())
            {
                var node = pairIter.Current;
                var handle = node->ObHandle;

                poster.Push(new Event.SelfInteraction { lhs = handle });
            }
        }

        public void Dispose()
        {
            _marshal.Dispose();
        }
    }
}
