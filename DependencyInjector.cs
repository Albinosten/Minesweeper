using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Security;


namespace Minesweeper
{
   public class DependencyInjector
    {
        private Dictionary<Type, object> units;

        public DependencyInjector()
        {
            this.units = new Dictionary<Type, object>();
        }

           public T Resolve<T>()
           {
               return  (T)this.Create(typeof(T));
           }
           private object Create(Type type)
           {
                var results = new List<object>();
                
                if(this.units.TryGetValue(type, out var result))
                {
                    Console.WriteLine($"returned stored type : " + type);

                    return result;
                }

                try
                {
                    Console.WriteLine($"working on {type.Name} ");

                    ConstructorInfo[] constructorInfoObjs = type.GetConstructors();
                    if(constructorInfoObjs != null)
                    {
                        Console.WriteLine($"The constructor of {type.Name} is: ");
                        foreach(var constructorInfoObj in constructorInfoObjs)
                        {
                            var parameters = constructorInfoObj.GetParameters();
                            
                            var parameterObjects = new List<Object>();
                            foreach(var parameter in parameters)
                            {
                                Console.WriteLine(parameter.ParameterType);

                                var parameterObject = this.Create(parameter.ParameterType);

                                parameterObjects.Add(parameterObject);
                            }
                            results.Add(constructorInfoObj.Invoke(parameterObjects.ToArray()));

                            Console.WriteLine(constructorInfoObj.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Could not find constructor for type {type.Name}");
                    }
                }
                catch(ArgumentNullException e)
                {
                    Console.WriteLine("ArgumentNullException: " + e.Message);
                }
                catch(ArgumentException e)
                {
                    Console.WriteLine("ArgumentException: " + e.Message);
                }
                catch(SecurityException e)
                {
                    Console.WriteLine("SecurityException: " + e.Message);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }


            result = results.FirstOrDefault(x => x != null);
            this.units.TryAdd(type, result);

            if(result == null)
            {
                
            }

            return result;
        }
    }
}