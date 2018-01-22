using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoContainer
{
    public static class MDLog
    {
        private static readonly IDictionary<Type, Type> types = new Dictionary<Type, Type>();
        private static readonly IDictionary<Type, object> typeInstance = new Dictionary<Type, object>();

        public static void Register<TService, TServiceImpl>()
        {
            types[typeof(TService)] = typeof(TServiceImpl);
        }

        public static void Register<TService, TServiceImpl>(TServiceImpl instances)
        {
            typeInstance[typeof(TService)] = instances;
        }

       
        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public static object Resolve(Type service)
        {
           if (typeInstance.ContainsKey(service))
            {
                return typeInstance[service];
            }

            Type _ServiceImplementation = types[service];
            ConstructorInfo constructors = _ServiceImplementation.GetConstructors()[0];
            ParameterInfo[] _constructorparameter = constructors.GetParameters();
            if(_constructorparameter.Length == 0)
            {
                return Activator.CreateInstance(_ServiceImplementation);
            }

            List<object> _parameter = new List<object>(_constructorparameter.Length);
            foreach(ParameterInfo _param in _constructorparameter)
            {
                _parameter.Add(Resolve(_param.ParameterType));
            }

            return constructors.Invoke(_parameter.ToArray());
        }

    }
}
