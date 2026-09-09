using System.Text.Json;

namespace MyLittleRPG
{
    class ConfigHelper
    {

        /// <summary>
        /// Fetches a pokemon from the pokeapi
        /// </summary>
        /// <param name="idx">idx of the pokemon to fetch</param>
        /// <returns></returns>
        public async Task<Pokemon> GetPokemon(int idx)
        {
            string uri = $"https://pokeapi.co/api/v2/pokemon/{idx}";

            Console.WriteLine(uri);

            using HttpClient client = new HttpClient();

            string json = await client.GetStringAsync(uri);

            return JsonSerializer.Deserialize<Pokemon>(json);
        }
    }
}
