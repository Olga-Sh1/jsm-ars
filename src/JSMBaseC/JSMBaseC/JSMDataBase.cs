using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    /// <summary>Оболочка для объекта для применения ДСМ-метода</summary>
    public abstract class JSMDataBase<T> : ISignable
    {
        /// <summary>Идентификатор</summary>
        public String ID { get; set; }
        /// <summary>Объект</summary>
        public T Inner { get; set; }
        /// <summary>Знак</summary>
        public Signs Sign { get; set; }
        protected JSMDataBase() { }
        public JSMDataBase(T ob)
        {
            this.Inner = ob;
            //this.ID = ob.ToString();
        }
       
       /// <summary>Сходство</summary><typeparam name="T"></typeparam>
       /// <param name="other"></param>
       /// <returns></returns>
        public abstract JSMDataBase<T> Intersect(JSMDataBase<T> other);
        /// <summary>Сходство</summary><typeparam name="T"></typeparam>
        /// <param name="groups">Группы</param>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract JSMDataBase<T> Intersect(JSMDataBase<T> other, String[] groups);

        /// <summary>Разность</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract JSMDataBase<T> Difference(JSMDataBase<T> other);
        /// <summary>Проверка на сходство</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract Boolean IsEnclosed(JSMDataBase<T> other);
        /// <summary>Проверка, что объект нулевой</summary>
        public abstract Boolean IsEmpty { get; }
        /// <summary>Равенство</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract Boolean IsEqual(JSMDataBase<T> other);
        /// <summary>Равенство для ДСМ-метода</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract Boolean IsJSMEqual(JSMDataBase<T> other);
        /// <summary>Количество непустых признаков</summary>
        public abstract Int32 CountNonEmptyProps { get; }
        /// <summary>Сумма</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract JSMDataBase<T> Sum(JSMDataBase<T> other);

        public virtual void Complete() { }
    }
}
