using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dapper
{
    static class Nucleo
    {      
        public static IEnumerable<PropertyInfo> ObterPropriedadeChave(Type tipo)
        {
            var propriedades = tipo.ObterPropriedades("KeyAttribute");

            return propriedades.Any() ? propriedades : tipo.ObterPropriedades(p => p.Name == "Id");
        }

        public static void VerificarChave(IEnumerable<PropertyInfo> propriedades)
        {
            Protect.Against(!propriedades.Any(), "A entidade deve possuir pelo menos uma propriedade Id ou decorada com atributo [Key]");
        }

        public static void VerificarChave(object entidade)
        {
            var propriedades = ObterPropriedadeChave(entidade.GetType());

            VerificarChave(propriedades);
        }

        public static void VerificarChave<T>()
        {
            var propriedades = Nucleo.ObterPropriedadeChave(typeof(T));

            VerificarChave(propriedades);
        }

        public static void GerarComandoSelect(IEnumerable<PropertyInfo> propriedades)
        {
            propriedades = propriedades as IList<PropertyInfo> ?? propriedades.ToList();

            foreach (var p in propriedades)
            {

            }
        }
    }
}
