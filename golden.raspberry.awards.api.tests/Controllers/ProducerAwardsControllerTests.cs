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
using System.Net.Http;
using System.Threading.Tasks;

namespace golden.raspberry.awards.api.tests
{
    [TestFixture]
    public class ProducerAwardsControllerTests
    {
        private WebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        private IConfiguration _config;
        private AppDbContext _appDbContext;
        private SqliteConnection _sqliteConnection;

        private const string ApiUrlVersion = "v1/producerawards";

        [SetUp]
        public void Setup()
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
            _appDbContext.MovieAwardNominations.RemoveRange(_appDbContext.MovieAwardNominations);
            _appDbContext.SaveChanges();
            _factory = new WebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task Quando_ProcessarMassaDeDadosOriginal_Deve_Retornar_MinMaxIgualAUmEProdutorBoDerekNoMaxEMin()
        {
            //Arrange
            var expectedCount = 1;
            var expectedProducer = "Bo Derek";
            var expectedInterval = 6;
            var expectedPreviusWin = 1984;
            var expectedFollowingWin = 1990;

            var movieAwards = SeedHelper.GetMovieAwardNominations(";", ".\\Resources\\movielist.csv");
            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
            producerAwardResult.Max[0].Producer.Should().Be(expectedProducer);
            producerAwardResult.Max[0].Interval.Should().Be(expectedInterval);
            producerAwardResult.Max[0].PreviusWin.Should().Be(expectedPreviusWin);
            producerAwardResult.Max[0].FollowingWin.Should().Be(expectedFollowingWin);
            producerAwardResult.Min[0].Producer.Should().Be(expectedProducer);
            producerAwardResult.Min[0].Interval.Should().Be(expectedInterval);
            producerAwardResult.Min[0].PreviusWin.Should().Be(expectedPreviusWin);
            producerAwardResult.Min[0].FollowingWin.Should().Be(expectedFollowingWin);
        }

        [Test]
        public async Task Quando_ExistirProdutorComMaiorIntervaloEntreDoisPremiosEProdutorComMenorIntervaloEntreDoisPremios_Deve_Retornar_MinMaxIgualAUmEProdutor1NoMaxEProdutor2NoMin()
        {
            //Arrange
            var expectedCount = 1;
            var expectedMinMaxProducer1 = "Producer1";
            var expectedMinMaxProducer2 = "Producer2";

            var movieAwards = new List<MovieAwardNomination>()
            {
                new MovieAwardNomination(){Year=1900, Title="Title1", Studio="Studio1", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1910, Title="Title2", Studio="Studio2", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1915, Title="Title3", Studio="Studio3", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1920, Title="Title4", Studio="Studio4", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1925, Title="Title5", Studio="Studio5", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1930, Title="Title6", Studio="Studio6", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1935, Title="Title7", Studio="Studio7", Producer="Producer4", Winner=""},
                new MovieAwardNomination(){Year=1940, Title="Title8", Studio="Studio8", Producer="Producer4", Winner=""}
            };

            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
            producerAwardResult.Max[0].Producer.Should().Be(expectedMinMaxProducer1);
            producerAwardResult.Min[0].Producer.Should().Be(expectedMinMaxProducer2);
        }

        [Test]
        public async Task Quando_ExistirProdutorComMaiorIntervaloEntreDoisPremiosEProdutorComMenorIntervaloEntreDoisPremios_Deve_Retornar_MinMaxIgualAUmEProdutor1NoMaxEProdutor4NoMin()
        {
            //Arrange
            var expectedCount = 1;
            var expectedMinMaxProducer1 = "Producer1";
            var expectedMinMaxProducer4 = "Producer4";

            var movieAwards = new List<MovieAwardNomination>()
            {
                new MovieAwardNomination(){Year=1900, Title="Title1", Studio="Studio1", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1910, Title="Title2", Studio="Studio2", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1915, Title="Title3", Studio="Studio3", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1920, Title="Title4", Studio="Studio4", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1921, Title="Title5", Studio="Studio5", Producer="Producer3", Winner="yes"},
                new MovieAwardNomination(){Year=1930, Title="Title6", Studio="Studio6", Producer="Producer3", Winner="yes"},
                new MovieAwardNomination(){Year=1931, Title="Title7", Studio="Studio7", Producer="Producer4", Winner="yes"},
                new MovieAwardNomination(){Year=1932, Title="Title8", Studio="Studio8", Producer="Producer4", Winner="yes"}
            };

            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
            producerAwardResult.Max[0].Producer.Should().Be(expectedMinMaxProducer1);
            producerAwardResult.Min[0].Producer.Should().Be(expectedMinMaxProducer4);
        }

        [Test]
        public async Task Quando_BancoDeDadosVazio_Deve_Retornar_MinMaxIgualAZero()
        {
            //Arrange
            var expectedCount = 0;

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
        }

        [Test]
        public async Task Quando_ExistirSomenteProdutoresSemVitorias_Deve_Retornar_MinMaxIgualAZero()
        {
            //Arrange
            var expectedCount = 0;

            var movieAwards = new List<MovieAwardNomination>()
            {
                new MovieAwardNomination(){Year=1900, Title="Title1", Studio="Studio1", Producer="Producer1", Winner=""},
                new MovieAwardNomination(){Year=1905, Title="Title2", Studio="Studio2", Producer="Producer1", Winner=""},
                new MovieAwardNomination(){Year=1910, Title="Title3", Studio="Studio3", Producer="Producer2", Winner=""},
                new MovieAwardNomination(){Year=1915, Title="Title4", Studio="Studio4", Producer="Producer2", Winner=""},
                new MovieAwardNomination(){Year=1920, Title="Title5", Studio="Studio5", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1925, Title="Title6", Studio="Studio6", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1930, Title="Title7", Studio="Studio7", Producer="Producer4", Winner=""},
                new MovieAwardNomination(){Year=1935, Title="Title8", Studio="Studio8", Producer="Producer4", Winner=""}
            };

            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
        }

        [Test]
        public async Task Quando_ExistirProdutoresComApenasUmaVitoria_Deve_Retornar_MinMaxIgualAZero()
        {
            //Arrange
            var expectedCount = 0;

            var movieAwards = new List<MovieAwardNomination>()
            {
                new MovieAwardNomination(){Year=1900, Title="Title1", Studio="Studio1", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1905, Title="Title2", Studio="Studio2", Producer="Producer1", Winner=""},
                new MovieAwardNomination(){Year=1910, Title="Title3", Studio="Studio3", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1915, Title="Title4", Studio="Studio4", Producer="Producer2", Winner=""},
                new MovieAwardNomination(){Year=1920, Title="Title5", Studio="Studio5", Producer="Producer3", Winner="yes"},
                new MovieAwardNomination(){Year=1925, Title="Title6", Studio="Studio6", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1930, Title="Title7", Studio="Studio7", Producer="Producer4", Winner="yes"},
                new MovieAwardNomination(){Year=1935, Title="Title8", Studio="Studio8", Producer="Producer4", Winner=""}
            };

            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
        }

        [Test]
        public async Task Quando_ExistirApenasUmProdutorComMaisDeUmaVitoria_Deve_Retornar_MinMaxIgualAUmEProdutor1NoMaxEMin()
        {
            //Arrange
            var expectedCount = 1;
            var expectedProducer = "Producer1";
            var expectedInterval = 5;
            var expectedPreviusWin = 1900;
            var expectedFollowingWin = 1905;

            var movieAwards = new List<MovieAwardNomination>()
            {
                new MovieAwardNomination(){Year=1900, Title="Title1", Studio="Studio1", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1905, Title="Title2", Studio="Studio2", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1910, Title="Title3", Studio="Studio3", Producer="Producer2", Winner=""},
                new MovieAwardNomination(){Year=1915, Title="Title4", Studio="Studio4", Producer="Producer2", Winner=""},
                new MovieAwardNomination(){Year=1920, Title="Title5", Studio="Studio5", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1925, Title="Title6", Studio="Studio6", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1930, Title="Title7", Studio="Studio7", Producer="Producer4", Winner=""},
                new MovieAwardNomination(){Year=1935, Title="Title8", Studio="Studio8", Producer="Producer4", Winner=""}
            };

            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
            producerAwardResult.Max[0].Producer.Should().Be(expectedProducer);
            producerAwardResult.Max[0].Interval.Should().Be(expectedInterval);
            producerAwardResult.Max[0].PreviusWin.Should().Be(expectedPreviusWin);
            producerAwardResult.Max[0].FollowingWin.Should().Be(expectedFollowingWin);
            producerAwardResult.Min[0].Producer.Should().Be(expectedProducer);
            producerAwardResult.Min[0].Interval.Should().Be(expectedInterval);
            producerAwardResult.Min[0].PreviusWin.Should().Be(expectedPreviusWin);
            producerAwardResult.Min[0].FollowingWin.Should().Be(expectedFollowingWin);
        }

        [Test]
        public async Task Quando_ExistirDoisProdutoresComMaisDeUmaVitoriaEIntervalosIguais_Deve_Retornar_MinMaxIgualADoisEProdutor1EProdutor2NoMaxEMin()
        {
            //Arrange
            var expectedCount = 2;
            var expectedMinMaxProducer1 = "Producer1";
            var expectedMinMaxProducer2 = "Producer2";

            var movieAwards = new List<MovieAwardNomination>()
            {
                new MovieAwardNomination(){Year=1900, Title="Title1", Studio="Studio1", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1905, Title="Title2", Studio="Studio2", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1910, Title="Title3", Studio="Studio3", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1915, Title="Title4", Studio="Studio4", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1920, Title="Title5", Studio="Studio5", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1925, Title="Title6", Studio="Studio6", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1930, Title="Title7", Studio="Studio7", Producer="Producer4", Winner=""},
                new MovieAwardNomination(){Year=1935, Title="Title8", Studio="Studio8", Producer="Producer4", Winner=""}
            };

            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
            producerAwardResult.Max[0].Producer.Should().Be(expectedMinMaxProducer1);
            producerAwardResult.Max[1].Producer.Should().Be(expectedMinMaxProducer2);
            producerAwardResult.Min[0].Producer.Should().Be(expectedMinMaxProducer1);
            producerAwardResult.Min[1].Producer.Should().Be(expectedMinMaxProducer2);
        }

        [Test]
        public async Task Quando_ExistirDoisOuMaisProdutoresComMaisDeUmaVitoriaEIntervalosIguais_Deve_Retornar_MinMaxIgualAQuantidadeDeProdutoresVencedoresEProdutoresNoMaxEMin()
        {
            //Arrange
            var expectedCount = 3;
            var expectedMinMaxProducer1 = "Producer1";
            var expectedMinMaxProducer2 = "Producer2";
            var expectedMinMaxProducer4 = "Producer4";

            var movieAwards = new List<MovieAwardNomination>()
            {
                new MovieAwardNomination(){Year=1900, Title="Title1", Studio="Studio1", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1905, Title="Title2", Studio="Studio2", Producer="Producer1", Winner="yes"},
                new MovieAwardNomination(){Year=1910, Title="Title3", Studio="Studio3", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1915, Title="Title4", Studio="Studio4", Producer="Producer2", Winner="yes"},
                new MovieAwardNomination(){Year=1920, Title="Title5", Studio="Studio5", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1925, Title="Title6", Studio="Studio6", Producer="Producer3", Winner=""},
                new MovieAwardNomination(){Year=1930, Title="Title7", Studio="Studio7", Producer="Producer4", Winner="yes"},
                new MovieAwardNomination(){Year=1935, Title="Title8", Studio="Studio8", Producer="Producer4", Winner="yes"}
            };

            _appDbContext.MovieAwardNominations.AddRange(movieAwards);
            _appDbContext.SaveChanges();

            //Act
            var response = await _client.GetAsync($"{ApiUrlVersion}");
            response.EnsureSuccessStatusCode();

            var producerAwardResult = JsonConvert.DeserializeObject<ProducerAwardResult>(response.Content.ReadAsStringAsync().Result);

            //Assert
            producerAwardResult.Max.Count.Should().Be(expectedCount);
            producerAwardResult.Min.Count.Should().Be(expectedCount);
            producerAwardResult.Max[0].Producer.Should().Be(expectedMinMaxProducer1);
            producerAwardResult.Max[1].Producer.Should().Be(expectedMinMaxProducer2);
            producerAwardResult.Max[2].Producer.Should().Be(expectedMinMaxProducer4);
            producerAwardResult.Min[0].Producer.Should().Be(expectedMinMaxProducer1);
            producerAwardResult.Min[1].Producer.Should().Be(expectedMinMaxProducer2);
            producerAwardResult.Min[2].Producer.Should().Be(expectedMinMaxProducer4);
        }

        [TearDown]
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