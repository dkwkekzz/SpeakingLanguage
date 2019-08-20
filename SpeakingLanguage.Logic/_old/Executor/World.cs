using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Executor
{
    internal static class World
    {
        struct Constants
        {
            public const int WORLD = 1;
            public const int UI = 2;
        }

        public static unsafe void Execute(slObject* src, slObject* dest)
        {
            var pos = src->GetState<Entity.Position>();
            var obs = src->GetState<Entity.Observer>();
            if (null == pos || null == obs)
                return;
            
            var tgPos = dest->GetState<Entity.Position>();
            if (null == tgPos)
                return;

            if (pos->layer == Constants.WORLD &&
                tgPos->layer == Constants.WORLD)
            {   // world
                var ofsx = pos->x - tgPos->x;
                var ofsy = pos->y - tgPos->y;
                var dist = obs->distance;
                if (dist * dist > ofsx * ofsx + ofsy * ofsy)
                {

                }
            }
        }

        //private static unsafe void _interactOnDetect(slObject* here, Entity.Detection* det, int ofsX, int ofsY)
        //{
        //    var inter = here->GetEntity<Entity.Interaction>();
        //    while (null != inter)
        //    {
        //        var disX = ofsX + inter->ofs_d_x + inter->dis_x;
        //        var disY = ofsY + inter->ofs_d_y + inter->dis_y;
        //        if (det->disX > disX && det->disY > disY)
        //        {
        //            // interact src x inter->dest
        //            // ...
        //
        //            _interactOnDetect(inter->dest, det, disX + inter->ofs_d_x, disY + inter->ofs_d_y);
        //        }
        //
        //        inter = here->NextEntity<Entity.Interaction>();
        //    }
        //}

        //private unsafe static void _popTransformAndExchangeCell(Locator terminal)
        //{
        //    var cellDic = terminal.CellDictionary;
        //    var requester = terminal.FrontStack.GetRequester<Event.Transform>();
        //    while (!requester.IsEmpty)
        //    {
        //        var tr = requester.Pop();
        //        if (tr->layer == (int)Define.Layer.None)
        //        {
        //            if (!cellDic.TryGetValue(tr->handle, out sCellNode* node))
        //                Library.ThrowHelper.ThrowKeyNotFound($"not exist handle in _sbtCellNode: {tr->handle.ToString()}");
        //
        //            var pc = node->Parent;
        //            if (null != pc)
        //            {
        //                pc->Remove(node);
        //                if (pc->Tail == null)
        //                {
        //                    cellDic.Remove(pc);
        //                }
        //            }
        //
        //            cellDic.Remove(node);
        //            return;
        //        }
        //
        //        if (tr->layer == (int)Define.Layer.World)
        //        {
        //            if (!cellDic.TryGetValue(tr->handle, out sCellNode* node))
        //            {
        //                node = cellDic.CreateCellNode();
        //                node->Take(tr->handle);
        //
        //                cellDic.Add(node);
        //            }
        //
        //            var prevCell = node->Parent;
        //
        //            // select cell
        //            sCell* nextCell = null;
        //            {
        //                var xi = tr->x >> sCell.BLOCK_BIT;
        //                var yi = tr->y >> sCell.BLOCK_BIT;
        //
        //                var handle = (xi << sCell.MAX_BIT) | yi;
        //
        //                if (!cellDic.TryGetValue(handle, out sCell* c))
        //                {
        //                    var rh = handle + (1 << sCell.MAX_BIT);
        //                    var bh = handle + 1;
        //                    var rbh = handle + (1 << sCell.MAX_BIT) + 1;
        //
        //                    cellDic.TryGetValue(rh, out sCell* rightCell);
        //                    cellDic.TryGetValue(bh, out sCell* bottomCell);
        //                    cellDic.TryGetValue(rbh, out sCell* rightBottomCell);
        //
        //                    c = cellDic.CreateCell();
        //                    c->Take(handle, rightCell, bottomCell, rightBottomCell);
        //
        //                    cellDic.Add(c);
        //                }
        //            }
        //
        //            if (prevCell != nextCell)
        //            {
        //                if (prevCell != null)
        //                    prevCell->Remove(node);
        //
        //                nextCell->Add(node);
        //            }
        //        }
        //    }
        //}
        //
        //private unsafe static void _pushSimpleInteraction(Locator terminal)
        //{
        //    var poster = terminal.FrontStack.GetPoster<Event.SimpleInteraction>();
        //
        //    var cellDic = terminal.CellDictionary;
        //    var nodeIter = cellDic.GetCellNodeEnumerator();
        //    while (nodeIter.MoveNext())
        //    {
        //        var node = nodeIter.Current;
        //        var handle = node->ObHandle;
        //
        //        var parentCell = node->Parent;
        //        var parentIter = parentCell->GetEnumerator();
        //        while (parentIter.MoveNext())
        //        {
        //            if (node == parentIter.Current)
        //                continue;
        //
        //            poster.Push(new Event.SimpleInteraction { lhs = handle, rhs = parentIter.Current->ObHandle });
        //        }
        //
        //        if (null != parentCell->RightCell)
        //        {
        //            var rightIter = parentCell->RightCell->GetEnumerator();
        //            while (rightIter.MoveNext())
        //            {
        //                poster.Push(new Event.SimpleInteraction { lhs = handle, rhs = parentIter.Current->ObHandle });
        //                poster.Push(new Event.SimpleInteraction { lhs = parentIter.Current->ObHandle, rhs = handle });
        //            }
        //        }
        //
        //        if (null != parentCell->BottomCell)
        //        {
        //            var bottomIter = parentCell->BottomCell->GetEnumerator();
        //            while (bottomIter.MoveNext())
        //            {
        //                poster.Push(new Event.SimpleInteraction { lhs = handle, rhs = parentIter.Current->ObHandle });
        //                poster.Push(new Event.SimpleInteraction { lhs = parentIter.Current->ObHandle, rhs = handle });
        //            }
        //        }
        //
        //        if (null != parentCell->RightBottomCell)
        //        {
        //            var rightBottomIter = parentCell->RightBottomCell->GetEnumerator();
        //            while (rightBottomIter.MoveNext())
        //            {
        //                poster.Push(new Event.SimpleInteraction { lhs = handle, rhs = parentIter.Current->ObHandle });
        //                poster.Push(new Event.SimpleInteraction { lhs = parentIter.Current->ObHandle, rhs = handle });
        //            }
        //        }
        //    }
        //}
        //
        //private unsafe static void _pushSelfInteraction(Locator terminal)
        //{
        //    var poster = terminal.FrontStack.GetPoster<Event.SelfInteraction>();
        //
        //    var cellDic = terminal.CellDictionary;
        //    var nodeIter = cellDic.GetCellNodeEnumerator();
        //    while (nodeIter.MoveNext())
        //    {
        //        var node = nodeIter.Current;
        //        var handle = node->ObHandle;
        //
        //        poster.Push(new Event.SelfInteraction { lhs = handle });
        //    }
        //}
    }
}
