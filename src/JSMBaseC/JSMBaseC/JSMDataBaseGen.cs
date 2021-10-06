using JSMBase.Medicine;
using JSMBase.SpecialAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    /// <summary>Объект для применения ДСМ-метода в общем виде: несколько групп признаков</summary>
    public sealed class JSMDataBaseGen : JSMDataBase<JSMMedicine>
    {
        /// <summary>Все оставшиеся признаки</summary>
        private BitArrayBase _Other;
        /// <summary>Непустые группы, по которым считается исчерпываемость</summary>
        private BitArrayBase[] _CompletenessGroups;
        /// <summary>Непустые группы, не входящие в исчерпываемость</summary>
        private BitArrayBase[] _NotEmptyGroups;
        private IExecContext<JSMMedicine> ctxt;

        private JSMDataBaseGen() { }
        public JSMDataBaseGen(JSMMedicine ob, IExecContext<JSMMedicine> ctxt):base(ob)
        {
            this.ctxt = ctxt;
            Fill();
        }

        #region JSMDataBase<JSMMedicine>
        public override JSMDataBase<JSMMedicine> Intersect(JSMDataBase<JSMMedicine> other)
        {
            JSMDataBaseGen other2 = other as JSMDataBaseGen;
            //пересекаем те, которые участуют в исчерпываемости
            var inters1 = _BulkInnerIntersect(this._CompletenessGroups, other2._CompletenessGroups);
            if (ctxt.CheckComplenessIsEmpty(inters1))
                return null;
            //пересекаем те, которые не участвуют в исчерпываемости
            var inters2 = _BulkInnerIntersect(this._NotEmptyGroups, other2._NotEmptyGroups);
            var inter3 = this._Other.Intersect(other2._Other);

            JSMDataBaseGen newEx = new JSMDataBaseGen()
            {
                _CompletenessGroups = inters1,
                _NotEmptyGroups = inters2,
                _Other = inter3,
                ctxt = ctxt
            };
            return newEx;
            //проверка, равны ли части, по которым ведем исчерпываемость
            //int length = _CompletenessGroups.Length;
            //for (int i = 0; i < length; i++)
            //{
            //    if (!_CompletenessGroups[i].Equals(inters1[i]))
            //        return IntersectResult.CreateNew;
            //}

            //length = _NotEmptyGroups.Length;
            //for (int i = 0; i < length; i++)
            //{
            //    if (!_NotEmptyGroups[i].Equals(inters2[i]))
            //        return IntersectResult.EqualsLeft;
            //}
            //return IntersectResult.Equals;
        }

        public override JSMDataBase<JSMMedicine> Difference(JSMDataBase<JSMMedicine> other)
        {
            throw new NotImplementedException();
        }

        public override bool IsEnclosed(JSMDataBase<JSMMedicine> other)
        {
            throw new NotImplementedException();
        }

        public override bool IsEmpty
        {
            get 
            {
                return ctxt.CheckComplenessIsEmpty(_CompletenessGroups)
                    && ctxt.CheckComplenessIsEmpty(_NotEmptyGroups)
                    && ctxt.CheckComplenessIsEmpty(new [] { _Other });
            }
        }

        public override int CountNonEmptyProps => throw new NotImplementedException();

        public override bool IsEqual(JSMDataBase<JSMMedicine> other)
        {
            JSMDataBaseGen other2 = other as JSMDataBaseGen;
            int length = _CompletenessGroups.Length;
            for (int i = 0; i < length; i++)
            {
                if (!_CompletenessGroups[i].Equals(other2._CompletenessGroups[i]))
                    return false;
            }
            length = _NotEmptyGroups.Length;
            for (int i = 0; i < length; i++)
            {
                if (!_NotEmptyGroups[i].Equals(other2._NotEmptyGroups[i]))
                    return false;
            }
            return true;
        }

        #endregion

        /// <summary>Пересечение группы признаков</summary><param name="thisarr">Первая группа</param><param name="other">Вторая группа</param><returns>Пересечение : NULL если пустота</returns>
        private static BitArrayBase[] _BulkInnerIntersect(BitArrayBase[] thisarr, BitArrayBase[] other)
        {
            int length = thisarr.Length;
            if (length != other.Length) throw new Exception("Неравное количество массивов в группе");
            BitArrayBase[] _Intersect = new BitArrayBase[length];
            for (int i = 0; i < length; i++)
            {
                BitArrayBase b = other[i].Intersect(thisarr[i]);
                _Intersect[i] = b;
            }
            return _Intersect;
        }

        private void Fill()
        {
            JSMBase datain = new JSMBase(Inner);
            _CompletenessGroups = ctxt.GetCompletenessGroups(datain.Arrays);
            _NotEmptyGroups = ctxt.GetInCompletenessGroups(datain.Arrays);
            _Other = ctxt.GetOther(datain.Arrays);
        }

        /// <summary>Равенство по группам исчерпываемости</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private bool EqualsLeft(JSMDataBaseGen other)
        {
            int length = _CompletenessGroups.Length;
            for (int i = 0; i < length; i++)
            {
                if (!_CompletenessGroups[i].Equals(other._CompletenessGroups[i]))
                    return false;
            }
            return true;
        }

        public override bool IsJSMEqual(JSMDataBase<JSMMedicine> other)
        {
            return EqualsLeft(other as JSMDataBaseGen);
        }

        public void CreateBody()
        {
            JSMBase jb = new JSMBase(ID, ctxt.Create(_CompletenessGroups, _NotEmptyGroups, _Other));
            jb.ConvertFromBitArray();
            Inner = jb.Inner;
        }

        public override JSMDataBase<JSMMedicine> Intersect(JSMDataBase<JSMMedicine> other, string[] groups)
        {
            throw new NotImplementedException();
        }

        public override JSMDataBase<JSMMedicine> Sum(JSMDataBase<JSMMedicine> other)
        {
            throw new NotImplementedException();
        }
    }
}
