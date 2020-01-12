using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreinaWeb.CadastroPessoas.Dominio;
using TreinaWeb.CadastroPessoas.Persistencia.EF;

namespace TreinaWeb.CadastroPessoas.Repositorio
{
    public class PessoaRepositorio : IRepositorio<Pessoa>
    {
        public List<Pessoa> SelecionarTodos()
        {
            CadastroPessoasDbContext contexto = new CadastroPessoasDbContext();
            List<Pessoa> pessoas = contexto.Pessoas.OrderBy(o => o.Nome).ToList();
            contexto.Dispose();
            return pessoas;
        }

        public int Adicionar(Pessoa objeto)
        {
            CadastroPessoasDbContext contexto = new CadastroPessoasDbContext();
            contexto.Pessoas.Add(objeto);
            return contexto.SaveChanges();            
        }
        
    }
}
