using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreinaWeb.CadastroPessoas.Dominio;
using TreinaWeb.CadastroPessoas.Repositorio;

namespace TreinaWeb.CSharpAvancado.CadastroPessoas
{
    public partial class FrmPessoas : Form
    {
        private List<Pessoa> _pessoas = new List<Pessoa>();
        //para usar a keyword lock é preciso passar de parametro um objeto privado estatico de apenas leitura
        //este servira como um semafaro para controlar o trafego de acesso ao objeto compartilhado
        private static readonly object locker = new Object();

        public FrmPessoas()
        {
            //para o form ser criado, ele utliza uma thread
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region aprendendo sobre tasks
            //criando uma thread para o PreencherDataGridView
            //Thread thread1 = new Thread(PreencherDataGridView); 
            //thread1.Start();

            //Exemplo 1, comente os outros exemplos para ver esse funcionar
            //criando uma Task, usamos o metodo Run para executa-la e o Run espera como parametro um delegate
            //do tipo Action (tipo Action não tem retorno), que pode ser escrito com lambda
            /*
            Task.Run(() => {
                Thread.Sleep(5000);
                IRepositorio<Pessoa> repositorioPessoas = new PessoaRepositorio();
                dgvPessoas.Invoke((MethodInvoker)delegate { 
                    dgvPessoas.DataSource = repositorioPessoas.SelecionarTodos();
                    dgvPessoas.Refresh();
                });
            });
            */

            //Exemplo 2.1, comente os outros exemplos para ver esse funcionar
            //O wait obriga o programa a esperar enquanto minhaTask não termina tudo que ela tem pra fazer
            //o problema é que o wait aqui está sendo chamado dentro do Form1_Load fazendo que não carregue a 
            //tela enquanto a minhaTask não termina, assim perdemos a utilidade de se usar Task/Thread
            /*
            Task minhaTask = Task.Run(() => {
                Thread.Sleep(5000);
                IRepositorio<Pessoa> repositorioPessoa = new PessoaRepositorio();
                _pessoas = repositorioPessoa.SelecionarTodos();
            });
            minhaTask.Wait();
            dgvPessoas.DataSource = _pessoas;
            dgvPessoas.Refresh();
            */

            //Exemplo 2.2, comente os outros exemplos para ver esse funcionar
            //Uma forma de resolver esse problema é com GetAwaiter, que é o objeto que será notificado quando
            //a minhaTask terminou de executar, temos o metodo OnCompleted que vai dizer o que deve ser feito
            //depois que minhaTask terminar, OnCompleted tambem recebe como paramentro um delegate do tipo Action
            /*
            Task minhaTask = Task.Run(() => {
                Thread.Sleep(5000);
                IRepositorio<Pessoa> repositorioPessoa = new PessoaRepositorio();
                _pessoas = repositorioPessoa.SelecionarTodos();
            });
            TaskAwaiter awaiter = minhaTask.GetAwaiter();
            //aqui nao precisa do Invoke para manipular o dgvPessoas porque quem vai alterar o dgvPessoas
            //é a thread do Form1_Load que é a qual o componente pertence
            awaiter.OnCompleted(() => {
                dgvPessoas.DataSource = _pessoas;
                dgvPessoas.Refresh();
            });
            */

            //Exemplo 2.3, comente os outros exemplos para ver esse funcionar
            //Outra forma de resolver esse problema é usando o ContinueWith que recebe como parametro a task retornada pelo metodo Run
            //e um metodo Action que essa nova task criada pelo ContinueWith tera que realizar assim que a primeira task terminar
            /*
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                IRepositorio<Pessoa> repositorioPessoa = new PessoaRepositorio();
                _pessoas = repositorioPessoa.SelecionarTodos();
            }).ContinueWith((taskAnterior) =>
            {
                //Como ContinueWith cria outra task temos que chamar o Invoke para manipular o dgvPessoas
                dgvPessoas.Invoke((MethodInvoker)delegate
                {
                    dgvPessoas.DataSource = _pessoas;
                    dgvPessoas.Refresh();
                });
            });
            */

            //Exemplo 3, comente os outros exemplos para ver esse funcionar
            //Retornando valores com Task, no inicio passamos qual o tipo de retorno
            /*
            Task<int>.Run(() =>
            {
                Thread.Sleep(5000);
                IRepositorio<Pessoa> repositorioPessoa = new PessoaRepositorio();
                _pessoas = repositorioPessoa.SelecionarTodos();
                //temos que colocar o return no final do run como em metodos normais
                return _pessoas.Count;
            }).ContinueWith((taskAnterior) =>
            {                
                dgvPessoas.Invoke((MethodInvoker)delegate
                {
                    dgvPessoas.DataSource = _pessoas;
                    dgvPessoas.Refresh();
                });
                //Depois utilizando o Result conseguimos trazer esse valor
                MessageBox.Show(string.Format("Há {0} registros.", taskAnterior.Result));
            });
            */

            /*
            //Exemplo 4, comente os outros exemplos para ver esse funcionar
            //Exceções
            try
            {
                Task<int>.Run(() =>
                {
                    //forçando uma exceção
                    throw new Exception("Teste");
                    Thread.Sleep(5000);
                    IRepositorio<Pessoa> repositorioPessoa = new PessoaRepositorio();
                    _pessoas = repositorioPessoa.SelecionarTodos();
                    return _pessoas.Count;
                }).ContinueWith((taskAnterior) =>
                {
                    try
                    {
                        dgvPessoas.Invoke((MethodInvoker)delegate
                        {
                            dgvPessoas.DataSource = _pessoas;
                            dgvPessoas.Refresh();
                        });
                        //A task só arremesa a exceção para quem a chamou se voce utilizar o Result ou o Wait nela
                        MessageBox.Show(string.Format("Há {0} registros.", taskAnterior.Result));
                    }
                    //O compilador empacota as exceções das task em um AggregateException
                    //por isso que no retorno da mensagem vem como "um ou mais erros"
                    //para ver o erro da task mesmo ("Teste" nesse caso) é necessario
                    //acessar uma propriedade do AggregateException, o InnerExceptions
                    //que é uma lista com as exceções obtidas
                    catch (AggregateException ex)
                    {
                        foreach(Exception excecao in ex.InnerExceptions)
                        {
                            MessageBox.Show(excecao.Message);
                        }
                    }                    
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
            #endregion
            txbPesquisa.Text = string.Empty;
            //PreencherDataGridView();
            PreencherDataGridViewAsync();
        }

        //async informa que o método é assincrono, então como assincrono ele precisa 
        //aguardar o resultado de um task para ser terminado, por isso o uso do await
        private async void PreencherDataGridViewAsync()
        {
            int quantidadeLinhas = await PreencherDataGridView();
            MessageBox.Show(string.Format("Há {0} registros.", quantidadeLinhas));
            dgvPessoas.Invoke((MethodInvoker)delegate {
                dgvPessoas.DataSource = _pessoas;
                dgvPessoas.Refresh();            
            });
        }


        private Task<int> PreencherDataGridView()
        {
            #region aprendendo sobre thread
            /*
            //Para exceções não adiantaria colocar um try catch aqui no metodo PreencherDataGridView, 
            //exceções devem ser tratadas dentro do metodo utlizado pelas threads
            Thread.Sleep(5000);
            Thread thread2 = new Thread(PreencherListaPessoas);
            Thread thread3 = new Thread(PreencherListaPessoas2);
            thread2.Start();
            thread3.Start();
            //Join é usado para aguardar a finalização de uma thread, sem ele o Invoke chama o DataGridView
            //antes da thread conseguir preencher o DataGridView com a lista de pessoas do banco de dados
            thread2.Join();
            thread3.Join();

            //como foi a thread do InitializeComponent que criou o form e seus componentes (dgvPessoas)
            //a thread que criamos não pode alterar diretamente esses componentes, por isso o uso do Invoke
            dgvPessoas.Invoke((MethodInvoker)delegate 
            {
                dgvPessoas.DataSource = _pessoas;
                dgvPessoas.Refresh();
            });
            */
            #endregion

            return Task<int>.Run(() =>
            {
                Thread.Sleep(2000);
                IRepositorio<Pessoa> repositorioPessoa = new PessoaRepositorio();
                _pessoas = repositorioPessoa.SelecionarTodos();
                return _pessoas.Count;
            });
        }

        private void PreencherListaPessoas()
        {
            IRepositorio<Pessoa> repositorioPessoas = new PessoaRepositorio();
            List<Pessoa> pessoas = repositorioPessoas.SelecionarTodos();
            //Use a instrução lock para sincronizar o acesso de thread com um recurso compartilhado
            lock (locker) //impede que ambas as thread manipulem o mesmo objeto (_pessoas) ao mesmo tempo
            {
                foreach (Pessoa p in pessoas)
                {
                    p.Nome += " Thread 2 ";
                    _pessoas.Add(p);
                    Thread.Sleep(300);
                }
            }            
        }

        private void PreencherListaPessoas2()
        {
            //O local correto para se tratar exceções em thread é o metodo usado pela thread, não onde ela fica
            try
            {
                //descomente a linha abaixo para ver como e possivel pegar exceção com thread
                //throw new Exception("Teste");
                IRepositorio<Pessoa> repositorioPessoas = new PessoaRepositorio();
                List<Pessoa> pessoas = repositorioPessoas.SelecionarTodos();
                //Use a instrução lock para sincronizar o acesso de thread com um recurso compartilhado
                lock (locker) //impede que ambas as thread manipulem o mesmo objeto (_pessoas) ao mesmo tempo
                {
                    foreach (Pessoa p in pessoas)
                    {
                        p.Nome += " Thread 3 ";
                        _pessoas.Add(p);
                        Thread.Sleep(100);
                    }
                }
            }
            catch(Exception ex)
            {
                ExibirErro(ex);
            }           
        }

        private void ExibirErro(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        private void btnAdicionarPessoa_Click(object sender, EventArgs e)
        {
            FrmAdicionarPessoa frmAdicionarPessoa = new FrmAdicionarPessoa();
            frmAdicionarPessoa.ShowDialog();
            //PreencherDataGridView();
            PreencherDataGridViewAsync();
        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            IRepositorio<Pessoa> repositorioPessoas = new PessoaRepositorio();
            dgvPessoas.DataSource = repositorioPessoas.Selecionar(pessoa => pessoa.Nome.Contains(txbPesquisa.Text));
            dgvPessoas.Refresh();
        }
    }
}
