using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WebAPI.WebApp.Api
{
    /// <summary>
    /// Herdando a classe da ControllerBase, não é possível trabalhar mais com
    /// a resposta do tipo Json diretamente como é feita na Controller do html
    /// tampouco com as views
    /// 
    /// Necessário criar uma anotação [ApiController] indicando que esta 
    /// controller é do tipo api
    /// 
    /// Necessário também criar uma anotação do tipo route [Route("rota")] 
    /// onde eu indico tanto as rotas básicas necessárias quanto também
    /// o id que preciso passar para as Actions que necessitam dela
    /// 
    /// As actions que não tem rotas definidas estão apenas utilizando
    /// a rota definida direto na controller, fazendo suas ações de acordo
    /// com o tipo de verbo e o conteúdo da request body
    /// 
    /// Nas actions que pedem conteúdo de request body, eu obrigatoriamente
    /// devo passar uma anotação [FromBody] indicando que virá paramêtros
    /// do body da requisição
    /// </summary>

    [ApiController]
    [Route("[controller]")] // aqui a rota está pegando automaticamente o nome do controlador
    public class LivrosController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        [Route("{id}")] // aqui estou setando o id sendo passado como parametro da url
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model.ToModel());
        }

        [HttpGet("{id}/capa")]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }


        [HttpGet]
        public IActionResult ListaDeLivros()
        {
            var lista = _repo.All.Select(l => l.ToModel()).ToList();
            return Ok(lista);
        }

        [HttpPost]
        public IActionResult Incluir([FromBody] LivroUpload model)
        {
           if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro);
            }
            return BadRequest();
        }

        [HttpPut]
        public IActionResult Alterar([FromBody]LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok(livro);
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("{id}")] // mesma coisa acima
        public IActionResult Excluir(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return NoContent();
        }
    }
}
