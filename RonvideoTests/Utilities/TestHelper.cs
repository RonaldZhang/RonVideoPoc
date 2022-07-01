using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RonvideoTests.Utilities
{
    public static class TestHelper
    {
        public static  TReturn CallInstancePrivateMethod<TInstance, TReturn>(TInstance instance, string methodName, object[] parameters)
        {
            Type type = instance.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo method = type.GetMethod(methodName, bindingAttr);

            return (TReturn)method.Invoke(instance, parameters);
        }

        public static TReturn CallStaticPrivateMethod<TInput, TReturn>(TInput instance, string methodName, object[] parameters)
        {
            Type type = instance as Type;

            BindingFlags bindingAttr = BindingFlags.NonPublic| BindingFlags.Static;

            MethodInfo method = type.GetMethod(methodName, bindingAttr);
     
            return (TReturn)method.Invoke(null, parameters);
        }
    }
}
