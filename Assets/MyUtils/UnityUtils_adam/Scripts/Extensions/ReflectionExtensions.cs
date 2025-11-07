using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UnityUtils
{
    public static class ReflectionExtensions
    {
        // 预定义类型显示名称字典
        static readonly Dictionary<Type, string> TypeDisplayNames = new() {
            { typeof(int), "int" },
            { typeof(float), "float" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(string), "string" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(uint), "uint" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(char), "char" },
            { typeof(object), "object" }
        };

        // 值元组类型定义
        static readonly Type[] ValueTupleTypes = {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>)
        };

        // 基本类型转换层次结构
        static readonly Type[][] PrimitiveTypeCastHierarchy = {
            new[] { typeof(byte), typeof(sbyte), typeof(char) },
            new[] { typeof(short), typeof(ushort) },
            new[] { typeof(int), typeof(uint) },
            new[] { typeof(long), typeof(ulong) },
            new[] { typeof(float) },
            new[] { typeof(double) }
        };

        /// <summary>
        /// 返回给定类型的默认值
        /// </summary>
        /// <param name="type">要获取默认值的类型</param>
        /// <returns>具有默认值的类型实例，如果类型是引用类型则为null</returns>
        public static object Default(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// 确定类型是否可从指定的泛型类型参数赋值
        /// </summary>
        /// <typeparam name="T">要检查的类型</typeparam>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果指定类型可从泛型类型参数T赋值则为true，否则为false</returns>
        public static bool Is<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// 确定类型是否为委托
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果类型是委托则为true，否则为false</returns>
        public static bool IsDelegate(this Type type) => typeof(Delegate).IsAssignableFrom(type);

        /// <summary>
        /// 确定类型是否为强类型委托
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>如果类型是强类型委托则为true，否则为false</returns>
        public static bool IsStrongDelegate(this Type type)
        {
            if (!type.IsDelegate()) return false;

            return !type.IsAbstract;
        }

        /// <summary>
        /// 确定字段是否为委托
        /// </summary>
        /// <param name="fieldInfo">要检查的字段</param>
        /// <returns>如果字段是委托则为true，否则为false</returns>
        public static bool IsDelegate(this FieldInfo fieldInfo) => fieldInfo.FieldType.IsDelegate();

        /// <summary>
        /// 确定字段是否为强类型委托
        /// </summary>
        /// <param name="fieldInfo">要查询的字段</param>
        /// <returns>如果字段是强类型委托则为true，否则为false</returns>
        public static bool IsStrongDelegate(this FieldInfo fieldInfo) => fieldInfo.FieldType.IsStrongDelegate();

        /// <summary>
        /// 确定类型是否为给定非泛型类型的泛型类型
        /// </summary>
        /// <param name="genericType">要使用的类型</param>
        /// <param name="nonGenericType">要测试的非泛型类型</param>
        /// <returns>如果类型是非泛型类型的泛型类型则为true</returns>
        public static bool IsGenericTypeOf(this Type genericType, Type nonGenericType)
        {
            if (!genericType.IsGenericType) { return false; }

            return genericType.GetGenericTypeDefinition() == nonGenericType;
        }

        /// <summary>
        /// 确定类型是否为给定基类的派生类型
        /// </summary>
        /// <param name="type">此类型</param>
        /// <param name="baseType">要测试的基类型</param>
        /// <returns>如果类型是基类型的派生类型则为true</returns>
        public static bool IsDerivedTypeOf(this Type type, Type baseType) => baseType.IsAssignableFrom(type);

        /// <summary>
        /// 确定给定类型的对象是否可以转换为指定类型
        /// </summary>
        /// <param name="from">此对象</param>
        /// <param name="to">转换的目标类型</param>
        /// <param name="implicitly">如果只应考虑隐式转换</param>
        /// <returns>如果可以执行转换则为true</returns>
        public static bool IsCastableTo(this Type from, Type to, bool implicitly = false)
            => to.IsAssignableFrom(from) || from.HasCastDefined(to, implicitly);

        /// <summary>
        /// 确定两个类型之间是否定义了转换
        /// </summary>
        /// <param name="from">要检查转换定义的源类型</param>
        /// <param name="to">要检查转换定义的目标类型</param>
        /// <param name="implicitly">如果只应考虑隐式转换</param>
        /// <returns>如果类型之间定义了转换则为true，否则为false</returns>
        static bool HasCastDefined(this Type from, Type to, bool implicitly)
        {
            if ((!from.IsPrimitive && !from.IsEnum) || (!to.IsPrimitive && !to.IsEnum))
            {
                return IsCastDefined
                       (
                           to,
                           m => m.GetParameters()[0].ParameterType,
                           _ => from,
                           implicitly,
                           false
                       )
                       || IsCastDefined
                       (
                           from,
                           _ => to,
                           m => m.ReturnType, implicitly,
                           true
                       );
            }

            if (!implicitly)
            {
                return from == to || (from != typeof(bool) && to != typeof(bool));
            }

            IEnumerable<Type> lowerTypes = Enumerable.Empty<Type>();
            foreach (Type[] types in PrimitiveTypeCastHierarchy)
            {
                if (types.Any(t => t == to))
                {
                    return lowerTypes.Any(t => t == from);
                }

                lowerTypes = lowerTypes.Concat(types);
            }

            return false; // IntPtr, UIntPtr, Enum, Boolean
        }

        /// <summary>
        /// 确定两个类型之间是否定义了转换
        /// </summary>
        /// <param name="type">要检查转换定义的类型</param>
        /// <param name="baseType">从方法获取基类型的函数</param>
        /// <param name="derivedType">从方法获取派生类型的函数</param>
        /// <param name="implicitly">如果只应考虑隐式转换</param>
        /// <param name="lookInBase">是否应在基类层次结构中搜索转换定义</param>
        /// <returns>如果类型之间定义了转换则为true，否则为false</returns>
        static bool IsCastDefined(
            Type type,
            Func<MethodInfo, Type> baseType,
            Func<MethodInfo, Type> derivedType,
            bool implicitly,
            bool lookInBase
        )
        {
            // 设置绑定标志以搜索公共和静态方法，并可选择包括基类层次结构
            var flags = BindingFlags.Public | BindingFlags.Static | (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);

            // 使用指定的绑定标志从类型获取所有方法
            MethodInfo[] methods = type.GetMethods(flags);

            // 检查是否有方法是隐式或显式转换运算符，以及基类型是否可从派生类型赋值
            return methods.Where(m => m.Name == "op_Implicit" || (!implicitly && m.Name == "op_Explicit"))
                .Any(m => baseType(m).IsAssignableFrom(derivedType(m)));
        }

        /// <summary>
        /// 将对象动态转换为指定类型
        /// </summary>
        /// <param name="type">转换的目标类型</param>
        /// <param name="data">要转换的对象</param>
        /// <returns>动态转换后的对象</returns>
        public static object Cast(this Type type, object data)
        {
            if (type.IsInstanceOfType(data))
            {
                return data;
            }

            try
            {
                return Convert.ChangeType(data, type);
            }
            catch (InvalidCastException)
            {
                var srcType = data.GetType();
                var dataParam = Expression.Parameter(srcType, "data");
                Expression body = Expression.Convert(Expression.Convert(dataParam, srcType), type);

                var run = Expression.Lambda(body, dataParam).Compile();
                return run.DynamicInvoke(data);
            }
        }

        /// <summary>
        /// 确定给定方法是否为重写
        /// </summary>
        /// <param name="methodInfo">要检查的方法</param>
        /// <returns>如果方法是重写则为true，否则为false</returns>
        public static bool IsOverride(this MethodInfo methodInfo)
            => methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType;

        /// <summary>
        /// 检查提供者上是否存在指定的属性
        /// </summary>
        /// <typeparam name="T">要测试的属性</typeparam>
        /// <param name="provider">属性提供者</param>
        /// <param name="searchInherited">是否应搜索基类声明</param>
        /// <returns>如果属性存在则为true，否则为false</returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider, bool searchInherited = true) where T : Attribute
        {
            try
            {
                return provider.IsDefined(typeof(T), searchInherited);
            }
            catch (MissingMethodException)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取给定类型的格式化显示名称
        /// </summary>
        /// <param name="type">要生成显示名称的类型</param>
        /// <param name="includeNamespace">生成类型名时是否应包括命名空间</param>
        /// <returns>生成的显示名称</returns>
        public static string GetDisplayName(this Type type, bool includeNamespace = false)
        {
            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            if (type.IsArray)
            {
                int rank = type.GetArrayRank();
                string innerTypeName = GetDisplayName(type.GetElementType(), includeNamespace);
                return $"{innerTypeName}[{new string(',', rank - 1)}]";
            }

            if (TypeDisplayNames.TryGetValue(type, out string baseName1))
            {
                if (!type.IsGenericType || type.IsConstructedGenericType)
                    return baseName1;
                Type[] genericArgs = type.GetGenericArguments();
                return $"{baseName1}<{new string(',', genericArgs.Length - 1)}>";

            }

            if (type.IsGenericTypeOf(typeof(Nullable<>)))
            {
                var innerType = type.GetGenericArguments()[0];
                return $"{innerType.GetDisplayName()}?";
            }

            if (type.IsGenericType)
            {
                var baseType = type.GetGenericTypeDefinition();
                Type[] genericArgs = type.GetGenericArguments();

                if (ValueTupleTypes.Contains(baseType))
                {
                    return GetTupleDisplayName(type, includeNamespace);
                }

                if (type.IsConstructedGenericType)
                {
                    var genericNames = new string[genericArgs.Length];
                    for (var i = 0; i < genericArgs.Length; i++)
                    {
                        genericNames[i] = GetDisplayName(genericArgs[i], includeNamespace);
                    }

                    string baseName = GetDisplayName(baseType, includeNamespace).Split('<')[0];
                    return $"{baseName}<{string.Join(", ", genericNames)}>";
                }

                string typeName = includeNamespace
                    ? type.FullName
                    : type.Name;

                return $"{typeName?.Split('`')[0]}<{new string(',', genericArgs.Length - 1)}>";
            }

            var declaringType = type.DeclaringType;
            if (declaringType == null)
            {
                return includeNamespace
                    ? type.FullName
                    : type.Name;
            }

            string declaringName = GetDisplayName(declaringType, includeNamespace);
            return $"{declaringName}.{type.Name}";

        }

        /// <summary>
        /// 获取元组类型的格式化显示名称
        /// </summary>
        /// <param name="type">要生成显示名称的元组类型</param>
        /// <param name="includeNamespace">生成类型名时是否应包括命名空间</param>
        /// <returns>元组类型的生成显示名称</returns>
        static string GetTupleDisplayName(this Type type, bool includeNamespace = false)
        {
            IEnumerable<string> parts = type
                .GetGenericArguments()
                .Select(x => x.GetDisplayName(includeNamespace));

            return $"({string.Join(", ", parts)})";
        }

        /// <summary>
        /// 确定来自不同类型的两方法是否具有相同的签名
        /// </summary>
        /// <param name="a">第一个方法</param>
        /// <param name="b">第二个方法</param>
        /// <returns><c>true</c> 如果它们相等</returns>
        public static bool AreMethodsEqual(MethodInfo a, MethodInfo b)
        {
            if (a.Name != b.Name) return false;

            ParameterInfo[] paramsA = a.GetParameters();
            ParameterInfo[] paramsB = b.GetParameters();

            if (paramsA.Length != paramsB.Length) return false;
            for (var i = 0; i < paramsA.Length; i++)
            {
                var pa = paramsA[i];
                var pb = paramsB[i];

                if (pa.Name != pb.Name) return false;
                if (pa.HasDefaultValue != pb.HasDefaultValue) return false;

                var ta = pa.ParameterType;
                var tb = pb.ParameterType;

                if (ta.ContainsGenericParameters || tb.ContainsGenericParameters)
                    continue;
                if (ta != tb) return false;
            }

            if (a.IsGenericMethod != b.IsGenericMethod) return false;

            if (!a.IsGenericMethod || !b.IsGenericMethod) return true;
            {
                Type[] genericA = a.GetGenericArguments();
                Type[] genericB = b.GetGenericArguments();

                if (genericA.Length != genericB.Length) return false;
                for (var i = 0; i < genericA.Length; i++)
                {
                    var ga = genericA[i];
                    var gb = genericB[i];

                    if (ga.Name != gb.Name) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 通过找到具有相等签名的对应方法，将方法重新基于到新类型上
        /// </summary>
        /// <param name="method">要重新基于的方法</param>
        /// <param name="newBase">要重新基于方法的新类型</param>
        /// <returns>重新基于后的方法</returns>
        public static MethodInfo RebaseMethod(this MethodInfo method, Type newBase)
        {
            var flags = BindingFlags.Default;

            flags |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;

            flags |= method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;

            MethodInfo[] candidates = newBase.GetMethods(flags)
                .Where(x => AreMethodsEqual(x, method))
                .ToArray();

            if (candidates.Length == 0)
            {
                throw new ArgumentException($"无法将方法 {method} 重新基于到类型 {newBase} 上，因为未找到匹配的候选方法");
            }

            if (candidates.Length > 1)
            {
                throw new ArgumentException($"无法将方法 {method} 重新基于到类型 {newBase} 上，因为找到太多匹配的候选方法");
            }

            return candidates[0];
        }
    }
}