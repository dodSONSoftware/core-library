using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides methods to facilitate instantiating types.
    /// </summary>
    public static class InstantiationHelper
    {
        #region Invokers
        /// <summary>
        /// Attempts to execute the default constructor for the <paramref name="onType"/>.
        /// </summary>
        /// <param name="onType">The type to attempt to instantiate.</param>
        /// <returns>The instantiated <paramref name="onType"/>.</returns>
        public static object InvokeDefaultCtor(Type onType)
        {
            var ctor = onType.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public /*| System.Reflection.BindingFlags.NonPublic*/,
                                             null,
                                             Type.EmptyTypes,
                                             null);
            if (ctor == null)
            {
                throw new NotImplementedException($"Cannot find .Ctor() for the type {onType.FullName}.");
            }
            return ctor.Invoke(null);
        }
        /// <summary>
        /// Attempts to execute the constructor for the <paramref name="onType"/> which contains the single, specified argument.
        /// </summary>
        /// <param name="onType">The type to attempt to instantiate.</param>
        /// <param name="parameterType">The type of the argument.</param>
        /// <param name="parameter">The value of the argument.</param>
        /// <returns>The instantiated <paramref name="onType"/>.</returns>
        public static object InvokeCtor(Type onType,
                                        Type parameterType,
                                        object parameter) => InvokeCtor(onType, new Type[] { parameterType }, new object[] { parameter });
        /// <summary>
        /// Attempts to execute the constructor for the <paramref name="onType"/> which contains the specified arguments.
        /// </summary>
        /// <param name="onType">The type to attempt to instantiate.</param>
        /// <param name="parameterTypes">The types of arguments.</param>
        /// <param name="parameters">The values of the arguments.</param>
        /// <returns>The instantiated <paramref name="onType"/>.</returns>
        public static object InvokeCtor(Type onType,
                                        Type[] parameterTypes,
                                        object[] parameters)
        {
            var ctor = onType.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public /*| System.Reflection.BindingFlags.NonPublic*/,
                                             null,
                                             parameterTypes,
                                             null);
            if (ctor == null)
            {
                var list = new StringBuilder(1024);
                foreach (var item in parameterTypes)
                {
                    list.Append(item.FullName + ", ");
                }
                list.Length -= 2;
                throw new NotImplementedException($"Cannot find .Ctor({list.ToString()}) for the type {onType.FullName}.");
            }
            return ctor.Invoke(parameters);
        }
        #endregion
    }
}
