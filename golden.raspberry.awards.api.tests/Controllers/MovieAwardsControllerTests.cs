using FluentAssertions;
using golden.raspberry.awards.api.Domain.Entities;
using golden.raspberry.awards.api.Extensions;
using golden.raspberry.awards.api.Infra.Data.Context;
using golden.raspberry.awards.api.Infra.Settings;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace golden.raspberry.awards.api.tests.Controllers
{
    [TestFixture]
    public class MovieAwardsControllerTests
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        private IConfiguration _config;
        private AppDbContext _appDbContext;
        private SqliteConnection _sqliteConnection;

        [OneTimeSetUp]
        public void OnTypeSetup()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            var appSettings = _config.LoadSettings<AppSettings>("AppSettings");

            _sqliteConnection = new SqliteConnection(appSettings.ConnectionString);
            _sqliteConnection.Open();

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlite(_sqliteConnection);
            var options = builder.Options;
            _appDbContext = new AppDbContext(options);
            _appDbContext.Database.EnsureDeleted();
            _appDbContext.Database.EnsureCreated();

            var movieAwards = SeedHelper.GetMovieAwardNominations(";", ".\\Resources\\movielist.csv");
            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            _factory = new WebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
        }

        #region Get

        [Test]
        public async Task Quando_BuscarTudoEBuscarMassaDeDadosOriginal_Deve_Retornar_QuantidadeDeNomeacoesIgualADuzentosESeis()
        {
            //Arrange
            var expectedCount = 206;

            //Act
            var response = await _client.GetAsync("/movieawards");
            response.EnsureSuccessStatusCode();

            var movieAwardNominations = JsonConvert.DeserializeObject<List<MovieAwardNomination>>(response.Content.ReadAsStringAsync().Result);

            //Assert
            movieAwardNominations.Count.Should().Be(expectedCount);
        }

        [Test]
        public async Task Quando_BuscarPorIdNaoExistente_Deve_Retornar_NuloECodigoSemConteudo()
        {
            //Act
            var response = await _client.GetAsync("/movieawards/500");
            response.EnsureSuccessStatusCode();

            var movieAwardNomination = JsonConvert.DeserializeObject<MovieAwardNomination>(response.Content.ReadAsStringAsync().Result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            movieAwardNomination.Should().BeNull();
        }

        [Test]
        public async Task Quando_BuscarPorIdExistente_Deve_Retornar_SucessoENomeacao()
        {
            //Arrange
            var expectedId = 8;

            //Act
            var response = await _client.GetAsync("/movieawards/8");
            response.EnsureSuccessStatusCode();

            var movieAwardNomination = JsonConvert.DeserializeObject<MovieAwardNomination>(response.Content.ReadAsStringAsync().Result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            movieAwardNomination.Id.Should().Be(expectedId);
        }

        [Test]
        public async Task Quando_BuscarPorIdZero_Deve_Retornar_CodigoNaoEncontrado()
        {
            //Arrange

            //Act
            var response = await _client.GetAsync("/movieawards/0");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region Post

        [Test]
        public async Task Quando_InserirNomeacao_Deve_Retornar_SucessoEIdentificadorDaNomeacao()
        {
            //Arrange
            var expectedId = "207";
            var movieAwardNomination = new MovieAwardNomination() { Year = 1900, Title = "Title1", Studio = "Studio1", Producer = "Producer1", Winner = "" };
            var manJson = JsonConvert.SerializeObject(movieAwardNomination);
            var content = new StringContent(manJson, Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PostAsync("/movieawards", content);
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadAsStringAsync().Result;

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should().Be(expectedId);
        }

        #endregion

        #region Put

        [Test]
        public async Task Quando_AtualizarNomeacao_Deve_Retornar_SucessoENomeacaoAtualizada()
        {
            //Arrange
            var expectedId = 8;
            var expectedTitle = "TitleAtualizado8";
            var movieAwardNomination = new MovieAwardNomination() { Id = 8, Year = 2000, Title = "TitleAtualizado8", Studio = "StudioAtualizado8", Producer = "ProducerAtualizado8", Winner = "yes" };
            var manJson = JsonConvert.SerializeObject(movieAwardNomination);
            var content = new StringContent(manJson, Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PutAsync("/movieawards", content);
            response.EnsureSuccessStatusCode();

            var movieAwardNominationResult = JsonConvert.DeserializeObject<MovieAwardNomination>(response.Content.ReadAsStringAsync().Result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            movieAwardNominationResult.Id.Should().Be(expectedId);
            movieAwardNominationResult.Title.Should().Be(expectedTitle);
        }

        #endregion

        #region Delete

        [Test]
        public async Task Quando_DeletarPorId_Deve_Retornar_SemConteudo()
        {
            //Arrange

            //Act
            var response = await _client.DeleteAsync("/movieawards/1");
            response.EnsureSuccessStatusCode();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        #endregion

        [OneTimeTearDown]
        public void TearDown()
        {
            _factory.Dispose();
            _client.Dispose();
            _appDbContext.Dispose();

            if (_sqliteConnection.State == ConnectionState.Open)
            {
                _sqliteConnection.Close();
            }
            _sqliteConnection.Dispose();
        }
    }
}