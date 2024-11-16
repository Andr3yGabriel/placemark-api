# KmlApi

## Descrição

O projeto **KmlApi** é uma API desenvolvida em ASP.NET Core que permite a manipulação de arquivos KML (Keyhole Markup Language). A API fornece endpoints para obter e exportar placemarks (marcadores) de um arquivo KML.

## Funcionalidades

- **Obter Placemarks**: Endpoint para obter placemarks filtrados a partir de um arquivo KML.
- **Exportar KML**: Endpoint para exportar placemarks em um novo arquivo KML.

## Como Rodar

### Pré-requisitos

- .NET 6.0 SDK ou superior
- Visual Studio 2022 ou superior (opcional, mas recomendado)

### Passos

1. Clone o repositório:

   ```sh
   git clone https://github.com/Andr3yGabriel/placemark-api.git
   cd KmlApi
   ```

2. Restaure as dependências:

   ```sh
   dotnet restore
   ```

3. Compile o projeto:

   ```sh
   dotnet build
   ```

4. Execute o projeto:

   ```sh
   dotnet run
   ```

5. Acesse a documentação Swagger para explorar os endpoints:
   - [http](http://localhost:5224/swagger)
   - [https](https://localhost:7009/swagger)

## Endpoints

### GET /api/placemarks

Obtém uma lista de placemarks filtrados.

**Parâmetros de Query:**

- `cliente`: string
- `situacao`: string
- `bairro`: string
- `referencia`: string
- `ruaCruzamento`: string

### POST /api/placemarks/export

Exporta placemarks para um novo arquivo KML.

**Body:**

- `Placemark` (JSON)

## Arquivos Importantes

- **[Program.cs](Program.cs)**: Configuração e inicialização da aplicação.
- **[Controllers/PlacemarkController.cs](Controllers/PlacemarkController.cs)**: Controlador que define os endpoints da API.
- **[Services/KmlService.cs](Services/KmlService.cs)**: Serviço que contém a lógica para manipulação dos arquivos KML.
- **[Interfaces/IKmlService.cs](Interfaces/IKmlService.cs)**: Interface do serviço KML.

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

## Licença

Este projeto está licenciado sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
