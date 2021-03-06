﻿using CadastroPessoas.Persistencia.NHibernate.Maps;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TreinaWeb.CadastroPessoas.Dominio;
using TreinaWeb.CadastroPessoas.Persistencia.EF;

namespace TreinaWeb.CadastroPessoas.Repositorio
{
    public class PessoaRepositorio : IRepositorio<Pessoa>
    {
        #region NHibernate
        /*
         //Se for usar o NHibernate
        private ISessionFactory _sessionFactory;

        public PessoaRepositorio()
        {
            Configuration config = new Configuration();
            config.Configure(); 
            config.AddAssembly(typeof(Pessoa).Assembly);
            HbmMapping mapping = CreateMappings();
            config.AddDeserializedMapping(mapping, null);
            _sessionFactory = config.BuildSessionFactory();
        }

        private HbmMapping CreateMappings()
        {
            ModelMapper mapper = new ModelMapper();
            mapper.AddMapping(typeof(PessoaMap));
            return mapper.CompileMappingForAllExplicitlyAddedEntities();
        }
        */
        #endregion

        public List<Pessoa> SelecionarTodos()
        {
            //Se for usar o Entity Framework
             CadastroPessoasDbContext contexto = new CadastroPessoasDbContext();
             List<Pessoa> pessoas = contexto.Pessoas.OrderBy(o => o.Nome).ToList();
             contexto.Dispose();
             return pessoas;

            #region NHibernate
            /* 
             //Se for usar o NHibernate
            using (ISession sessao = _sessionFactory.OpenSession())
            {
                IQuery consulta = sessao.CreateQuery("FROM Pessoa");
                return consulta.List<Pessoa>().ToList();
            }
            */
            #endregion
        }

        public int Adicionar(Pessoa objeto)
        {
            //Se for usar o Entity Framework
             CadastroPessoasDbContext contexto = new CadastroPessoasDbContext();
             contexto.Pessoas.Add(objeto);
             return contexto.SaveChanges();

            #region NHibernate
            /*
            //Se for usar o NHibernate
            using (ISession sessao = _sessionFactory.OpenSession())
            {
                using(var transacao = sessao.BeginTransaction())
                {
                    sessao.Save(objeto);
                    transacao.Commit();
                    return 0;
                }
            }
            */
            #endregion
        }

        public async void AdicionarAsync(Pessoa objeto, Action<int> callBack)
        {
            CadastroPessoasDbContext contexto = new CadastroPessoasDbContext();
            contexto.Pessoas.Add(objeto);
            Thread.Sleep(2000);
            await contexto.SaveChangesAsync().ContinueWith((taskAnterior) => 
            {
                int linhasAfetadas = taskAnterior.Result;
                callBack(linhasAfetadas);
            });
        }
        //Utilizando paralelização com AsParallel
        public List<Pessoa> Selecionar(Func<Pessoa, bool> whereClause)
        {
            CadastroPessoasDbContext contexto = new CadastroPessoasDbContext();
            List<Pessoa> pessoas = contexto.Pessoas.AsParallel().Where(whereClause).ToList();
            contexto.Dispose();
            return pessoas;
        }
    }
}
