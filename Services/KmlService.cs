
using SharpKml.Dom;
using SharpKml.Engine;

public class KmlService : IKmlService
{
    private readonly string _kmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DIRECIONADORES1.KML");

    public KmlService()
    {
        _kmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DIRECIONADORES1.KML");
    }

    private async Task<KmlFile> LoadKmlFileAsync()
    {
        using var stream = File.OpenRead(_kmlPath);

        if (stream.Length == 0)
        {
            throw new InvalidOperationException("O arquivo KML está vazio.");
        }

        var kmlFile = await Task.Run(() => KmlFile.Load(stream));

        if (kmlFile == null || kmlFile.Root == null)
        {
            throw new InvalidOperationException("Falha ao carregar o arquivo KML.");
        }

        return kmlFile;
    }

    private IEnumerable<Placemark> ExtractPlacemarks(KmlFile kmlFile)
    {
        var placemarks = new List<Placemark>();

        if (kmlFile.Root is Kml kmlRoot && kmlRoot.Feature is Container rootContainer)
        {
            ExtractFeatures(rootContainer.Features, placemarks);
        }
        else if (kmlFile.Root is Document document)
        {
            ExtractFeatures(document.Features, placemarks);
        }
        else if (kmlFile.Root is Folder folder)
        {
            ExtractFeatures(folder.Features, placemarks);
        }

        return placemarks;
    }


    private void ExtractFeatures(IEnumerable<Feature> features, List<Placemark> placemarks)
    {
        foreach (var feature in features)
        {
            if (feature is SharpKml.Dom.Placemark kmlPlacemark)
            {
                placemarks.Add(ConvertKmlPlacemark(kmlPlacemark));
            }
            else if (feature is Container container)
            {
                ExtractFeatures(container.Features, placemarks);
            }
        }
    }

    private Placemark ConvertKmlPlacemark(SharpKml.Dom.Placemark kmlPlacemark)
    {
        return new Placemark
        {
            Cliente = ExtractData(kmlPlacemark, "CLIENTE"),
            Situacao = ExtractData(kmlPlacemark, "SITUAÇÃO"),
            Bairro = ExtractData(kmlPlacemark, "BAIRRO"),
            Referencia = ExtractData(kmlPlacemark, "REFERENCIA"),
            RuaCruzamento = ExtractData(kmlPlacemark, "RUA/CRUZAMENTO")
        };
    }

    private string ExtractData(SharpKml.Dom.Placemark placemark, string dataName)
    {
        if (placemark.ExtendedData != null)
        {
            foreach (var data in placemark.ExtendedData.Data)
            {
                if (data.Name.Equals(dataName, StringComparison.OrdinalIgnoreCase))
                {
                    return data.Value;
                }
            }
        }
        return string.Empty;
    }

    public async Task<IEnumerable<Placemark>> GetPlacemarksAsync(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento)
    {
        var kmlFile = await LoadKmlFileAsync();
        var placemarks = ExtractPlacemarks(kmlFile);

        var validClientes = placemarks.Select(p => p.Cliente).Distinct().ToList();
        var validSituacoes = placemarks.Select(p => p.Situacao).Distinct().ToList();
        var validBairros = placemarks.Select(p => p.Bairro).Distinct().ToList();

        if (!string.IsNullOrEmpty(cliente) && !validClientes.Contains(cliente, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Filtro 'CLIENTE' inválido.");
        }

        if (!string.IsNullOrEmpty(situacao) && !validSituacoes.Contains(situacao, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Filtro 'SITUAÇÃO' inválido.");
        }

        if (!string.IsNullOrEmpty(bairro) && !validBairros.Contains(bairro, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Filtro 'BAIRRO' inválido.");
        }

        if (!string.IsNullOrEmpty(referencia) && referencia.Length < 3)
        {
            throw new ArgumentException("Filtro 'REFERENCIA' deve ter pelo menos 3 caracteres.");
        }

        if (!string.IsNullOrEmpty(ruaCruzamento) && ruaCruzamento.Length < 3)
        {
            throw new ArgumentException("Filtro 'RUA/CRUZAMENTO' deve ter pelo menos 3 caracteres.");
        }

        if (!string.IsNullOrEmpty(cliente))
        {
            placemarks = placemarks.Where(p => p.Cliente.Equals(cliente, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(situacao))
        {
            placemarks = placemarks.Where(p => p.Situacao.Equals(situacao, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(bairro))
        {
            placemarks = placemarks.Where(p => p.Bairro.Equals(bairro, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(referencia))
        {
            placemarks = placemarks.Where(p => p.Referencia.Contains(referencia, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(ruaCruzamento))
        {
            placemarks = placemarks.Where(p => p.RuaCruzamento.Contains(ruaCruzamento, StringComparison.OrdinalIgnoreCase));
        }

        return placemarks;
    }

    public async Task<KmlFile> ExportKmlAsync(string cliente, string situacao, string bairro, string referencia, string ruaCruzamento)
    {
        var filteredPlacemarks = await GetPlacemarksAsync(cliente, situacao, bairro, referencia, ruaCruzamento);

        var document = new Document();
        foreach (var placemark in filteredPlacemarks)
        {
            var kmlPlacemark = new SharpKml.Dom.Placemark
            {
                Name = placemark.Cliente,
            };

            var extendedData = new ExtendedData();
            extendedData.AddData(new Data { Name = "RUA/CRUZAMENTO", Value = placemark.RuaCruzamento });
            extendedData.AddData(new Data { Name = "REFERENCIA", Value = placemark.Referencia });
            extendedData.AddData(new Data { Name = "BAIRRO", Value = placemark.Bairro });
            extendedData.AddData(new Data { Name = "SITUAÇÃO", Value = placemark.Situacao });
            extendedData.AddData(new Data { Name = "CLIENTE", Value = placemark.Cliente });

            kmlPlacemark.ExtendedData = extendedData;

            document.AddFeature(kmlPlacemark);
        }

        var kml = new Kml
        {
            Feature = document
        };

        var kmlFile = KmlFile.Create(kml, false);

        return kmlFile;
    }

    public async Task<FilterOptions> GetFilterOptionsAsync()
    {
        var kmlFile = await LoadKmlFileAsync();
        var placemarks = ExtractPlacemarks(kmlFile);

        var filterOptions = new FilterOptions
        {
            Clientes = placemarks.Select(p => p.Cliente).Where(c => !string.IsNullOrEmpty(c)).Distinct(),
            Situacoes = placemarks.Select(p => p.Situacao).Where(s => !string.IsNullOrEmpty(s)).Distinct(),
            Bairros = placemarks.Select(p => p.Bairro).Where(b => !string.IsNullOrEmpty(b)).Distinct()
        };

        return filterOptions;
    }
}