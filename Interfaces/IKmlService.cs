using SharpKml.Engine;

public interface IKmlService
{
    Task<IEnumerable<Placemark>> GetPlacemarksAsync(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento);
    Task<KmlFile> ExportKmlAsync(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento);
    Task<FilterOptions> GetFilterOptionsAsync();
}
