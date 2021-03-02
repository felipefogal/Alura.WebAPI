using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.WebAPI.WebApp.Formatters
{
    public class ListaLeituraCsvFormatter : TextOutputFormatter
    {
        public ListaLeituraCsvFormatter()
        {
            var textcsvMediaType = MediaTypeHeaderValue.Parse("text/csv");
            var appCsvMediaType = MediaTypeHeaderValue.Parse("application/csv");
            SupportedMediaTypes.Add(textcsvMediaType);
            SupportedMediaTypes.Add(appCsvMediaType);
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            return type == typeof(Lista);
        }
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var listaLeituraCsv = "";
            if (context.Object is Lista)
            {
                var lista = context.Object as Lista;
                var listaLivros = lista.Livros;
                listaLeituraCsv = $"{listaLivros.ToList()}";
            }

            using (var escritorListaLeitura = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return escritorListaLeitura.WriteAsync(listaLeituraCsv);
            }
        }
    }
}
