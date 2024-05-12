namespace PokemonsDesktop.Models;

public class PokemonParametersVM
{
    public int Id { get; set; }

        public decimal Height { get; set; }

        public decimal Weigh { get; set; }

        public Group SelectedGroup { get; set; }

        public int AllGame { get; set; }
        
        public decimal RarityProcent { get; set; }

        public int Shainy { get; set; }
        
        public int? HatchingTimeDay { get; set; }

        public int EvolutionStage { get; set; }
}