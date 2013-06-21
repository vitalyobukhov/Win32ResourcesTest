using System;

namespace ResourcesTest
{
    public static class TypeSwitch
    {
        public abstract class CaseInfo
        {
            public bool IsDefault { get; private set; }
            public Type Target { get; private set; }


            protected CaseInfo(Type target, bool isDefault = false)
            {
                if (!isDefault && target == null)
                    throw new ArgumentNullException("target");

                Target = target;
                IsDefault = isDefault;
            }
        }

        public sealed class DoInfo : CaseInfo
        {
            public Action<object> Do { get; private set; }


            public DoInfo(Type target, Action<object> @do, bool isDefault = false) :
                base(target, isDefault)
            {
                if (@do == null)
                    throw new ArgumentNullException("do");

                Do = @do;
            }
        }

        public sealed class ApplyInfo<TResult> : CaseInfo
        {
            public Func<object, TResult> Apply { get; private set; }


            public ApplyInfo(Type target, Func<object, TResult> apply, bool isDefault = false) :
                base(target, isDefault)
            {
                if (apply == null)
                    throw new ArgumentNullException("apply");

                Apply = apply;
            }
        }


        public static void Do(object source, params DoInfo[] cases)
        {
            foreach (var c in cases)
            {
                if (c.IsDefault || (source != null && source.GetType().IsAssignableFrom(c.Target)))
                {
                    c.Do(source);
                    break;
                }
            }
        }

        public static TResult Apply<TResult>(object source, params ApplyInfo<TResult>[] cases)
        {
            foreach (var c in cases)
            {
                if (c.IsDefault || (source != null && source.GetType().IsAssignableFrom(c.Target)))
                {
                    return c.Apply(source);
                }
            }

            return default(TResult);
        }

        public static DoInfo Case<T>(Action action)
        {
            return new DoInfo(typeof(T), x => action());
        }

        public static DoInfo Case<T>(Action<T> action)
        {
            return new DoInfo(typeof(T), x => action((T)x));
        }

        public static DoInfo Default(Action action)
        {
            return new DoInfo(null, x => action(), true);
        }

        public static ApplyInfo<TResult> Case<T, TResult>(Func<TResult> func)
        {
            return new ApplyInfo<TResult>(typeof(T), x => func());
        }

        public static ApplyInfo<TResult> Case<T, TResult>(Func<T, TResult> func)
        {
            return new ApplyInfo<TResult>(typeof(T), x => func((T)x));
        }

        public static ApplyInfo<TResult> Default<TResult>(Func<TResult> func)
        {
            return new ApplyInfo<TResult>(null, x => func(), true);
        }
    }
}
