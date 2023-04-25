using golden.raspberry.awards.api.Domain.Entities;
using golden.raspberry.awards.api.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace golden.raspberry.awards.api.Application.Services
{
    public interface IProducerAwardService
    {
        /// <summary>
        /// Retorna o resultado dos prêmios dos produtores.
        /// </summary>
        /// <returns>Duas listas com o(s) produtor(es) com maior e menor intervalo entre dois prêmios consecutivos</returns>
        ProducerAwardResult GetProducerAwardsResults();
    }

    public class ProducerAwardService : IProducerAwardService
    {
        private readonly IBaseRepository<MovieAwardNomination> _baseRepository;

        public ProducerAwardService(IBaseRepository<MovieAwardNomination> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public ProducerAwardResult GetProducerAwardsResults()
        {
            var producerNominations = GetProducerNominations();
            var producerAwardResult = new ProducerAwardResult()
            {
                Max = GetProducerAwardsMaxInterval(producerNominations),
                Min = GetProducerAwardsMinInterval(producerNominations)
            };

            return producerAwardResult;
        }

        /// <summary>
        /// Retorna a lista de nomeações dos filmes agrupadas por seus produtores.
        /// </summary>
        private Dictionary<string, List<ProducerNomination>> GetProducerNominations()
        {
            var movieAwards = _baseRepository.Select();
            var producerNominations = new Dictionary<string, List<ProducerNomination>>();

            //Cria um dicionário onde a chave é o produtor
            //Com a lista de nomeações contendo o ano de participação e se foi vencedor do prêmio.
            foreach (var movieAward in movieAwards)
            {
                var producers = movieAward.Producer.Replace(" and ", ",").Split(",");
                foreach (var pItem in producers)
                {
                    var producer = pItem.Trim();
                    var year = movieAward.Year;
                    var isWinner = movieAward.Winner.Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false;
                    if (!producerNominations.ContainsKey(producer))
                    {
                        producerNominations[producer] = new List<ProducerNomination>();
                    }
                    producerNominations[producer].Add(new ProducerNomination()
                    {
                        Year = year,
                        IsWinner = isWinner
                    });
                }
            }

            return producerNominations;
        }

        /// <summary>
        /// Retorna a lista de produtor(es) com maior intervalo entre dois prêmios consecutivos.
        /// </summary>
        /// <param name="producerNominations">Lista de nomeações dos filmes agrupadas por produtor.</param>
        private List<ProducerAward> GetProducerAwardsMaxInterval(Dictionary<string, List<ProducerNomination>> producerNominations)
        {
            var producerAwards = new List<ProducerAward>();
            var foundInterval = 0;
            foreach (var pn in producerNominations)
            {
                var years = pn.Value.OrderBy(x => x.Year).ToList();
                for (var i = 0; i < years.Count - 1; i++)
                {
                    if (!years[i + 1].IsWinner || !years[i].IsWinner) continue;
                    var interval = years[i + 1].Year - years[i].Year;
                    if (interval < foundInterval) continue;
                    if (producerAwards.Any(p => p.Interval < interval))
                    {
                        producerAwards.RemoveAll(p => p.Interval < interval);
                    }

                    producerAwards.Add(new ProducerAward()
                    {
                        Producer = pn.Key,
                        Interval = interval,
                        PreviusWin = years[i].Year,
                        FollowingWin = years[i + 1].Year
                    });
                    foundInterval = interval;
                }
            }
            return producerAwards;
        }

        private List<ProducerAward> GetProducerAwardsMinInterval(Dictionary<string, List<ProducerNomination>> producerNominations)
        {
            var producerAwards = new List<ProducerAward>();
            var foundInterval = int.MaxValue;
            foreach (var pn in producerNominations)
            {
                var years = pn.Value.OrderBy(x => x.Year).ToList();
                for (int i = 0; i < years.Count - 1; i++)
                {
                    if (!years[i + 1].IsWinner || !years[i].IsWinner) continue;
                    var interval = years[i + 1].Year - years[i].Year;
                    if (interval > foundInterval) continue;
                    if (producerAwards.Any(p => p.Interval > interval))
                    {
                        producerAwards.RemoveAll(p => p.Interval > interval);
                    }

                    producerAwards.Add(new ProducerAward()
                    {
                        Producer = pn.Key,
                        Interval = interval,
                        PreviusWin = years[i].Year,
                        FollowingWin = years[i + 1].Year
                    });
                    foundInterval = interval;
                }
            }
            return producerAwards;
        }
    }
}
