# WEB APPLICATION API

Voici la documentation complète de l'API : 
les processus en place et comment le maintenir, comment le lancer en mode test et effectuer des 
mise à jour avec la BDD.

# Sommaire

<!--TOC-->
- [Ajouter un contrôleur](#ajouter-un-controleur)
  - [Ajouter un EndPoint](#ajouter-un-endpoint)
    - [EndPoint sans retour](#endpoint-sans-retour)
    - [EndPoint avec retour (json)](#endpoint-avec-retour-json)
      - [Retour d'une donnée](#retour-dune-donnee)
      - [Retour de plusieurs données](#retour-de-plusieurs-donnees)
<!--/TOC-->

# Ajouter un contrôleur

Pour ajouter un contrôleur et qu'il soit fonctionnel au lancement de l'API :

1. Créer la classe en question dans le dossier **Controllers**

	💬 *La convention veut que chaque controller termine par le mot clé "**Controller**"*

2. Ajouter les anotations suivantes : 
    - `[ApiController]`
	- `[Produces("application/json")]`

3. Hériter le Controller de la class `FreshTechController`

4. `FreshTechController` a besoin d'une instance de `FTDbContext` dans son constructeur. Il faut
donc créer un constructeur du contrôleur avec en paramètre cette instance et le renvoyer avec `base`

Exemple :
```Csharp
namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ExampleController : FreshTechController
    {
        public ExampleController(FTDbContext dbContext) : base(dbContext) { }
    }
}
```

## Ajouter un EndPoint

1. Avant de créer le point d'entré, il faut d'abord créer une route. Dans **AppCore>Services>APIRoute** :
    - Créer une nouvelle variable constante de type `string` 
    - Avec le **nom de la variable** en **Majuscule**
    - Et la route avec des séparation `/` 

    💬 *<u>Convention :</u> <br>
    => **nom de la variable**, les mots sont séparées par `_` <br>
    => **la route**, respect le format suivant : `<nomClassSansLeMotController>/<nomDeLaVariable>`*

    Exemple: 
    ```Csharp
     public const string GET_TIME = "/time/getTime";
    ```

2. Créer une classe héritant du parent `EndPointArgs` dans le dossier **AppCore>Services>GeneralMessage>Args**
    
    💬 *<u>Convention :</u> <br>
    => **nom de la classe**, commence par `EP`*

3. `override` ensuite la fonction `Route()` avec la route précédemment créée.

4. **(Optionnel)** Si vous avez besoin de paramètre pour appeler le point d'entré, ajouter donc des variables
    à la classe en ajoutant l'annotation `[JsonInclude]` au dessus de chaque variable. Puis créer un constructeur
    avec tout les paramètres nécessaire.

5. Revenir sur le contrôleur, créer le point d'entré sous cette forme :<br>

    ```Csharp
    [HttpPost(/*renseigner le point d'entré ici*/)]
    public async Task<IActionResult> ExampleEndPoint(FTMessageClient message){
        // fonction nécessaire au bon fonctionnement de l'authentification
        return await ProcessAndCheckToken</*la classe du EndPoint ici*/>(message, (args) =>
        {
            // fonction à executer
        });
    }
    ```

6. La fonction à executer dans ce trouver dans `ProcessAndCheckToken` pour assurer le bon fonctionnement
   de la sécurité du système.

7. Effectuer les réponses avec les fonctions prévues :
    - `BadRequest()` // à utiliser en cas de problème avec les paramètres de l'utilisateurs ou autre problème mineur
    - `Ok()` // voir : EndPoint sans retour
    - `Json()` // voir : EndPoint avec retour
    - `Problem()` // à utiliser en cas de problème non prévue ou d'exceptions générer

    ⚠ *À l'utilisation de `BadRequest()` effectuer des retours en `string` en créant une variable
        dans `AppCore>Services>APIError` <br><br>
        Exemple:*

    ```Csharp
    // dans APIError
    public const string USER_ID_NOT_EXIST = "User id not exist";
    ```

**Exemple d'utilisation complète**

```Csharp
[HttpPost(APIRoute.GET_TIME)]
public async Task<IActionResult> GetTheTime(FTMessageClient message)
{
    return await ProcessAndCheckToken<EPGetTime>(message, (args) =>
    {
        // récupère l'userId par le UserGuid
        Guid? userId = GetUtilisateurIdByUserGuid(message.UserGuid);
        if (userId == null)
        {
            // exemple d'erreur
            return BadRequest(APIError.BAD_USER_TOKEN);
        }

        MyTime? time = dbContext.Times.FirstOrDefault(x => x.id == args.IdTime);

        if(time == null)
        {
            // exemple d'erreur
            return BadRequest(APIError.TIME_NOT_FOUND);
        }

        return Json(Message(message.UserGuid, time))
    });
}
```

### EndPoint sans retour

Si votre EndPoint n'a pas besoin de renvoyer des données alors utiliser la fonction `Ok()` pour effectuer un retour
normal. Ce qui signifi, du côté application, un appel de ce type : `FTMClientManager.SendRequest(EndPointArgs request)`

### EndPoint avec retour (json)

Si votre EndPoint a besoin de renvoyer des données alors utiliser la combinaison des fonctions `Json()` et 
`Message()`.

- `Message()` sert à créer une instance de `FTMessageServer` afin d'assurer le chiffrement et la signature 
   des données envoyés. Il demande en paramètre : 
    - **UserGuid** en `string`
    - **data** : les données renvoyées
- `Json()` : convertie le tout en JSON et génère une réponse. 

#### Retour d'une donnée

En cas de réponse avec un paramètre seulement, insérer les données directement dans `Message()` avec le 
paramètre `data`

#### Retour de plusieurs données

En cas de réponse avec plusieurs paramètres :

1. Créer dans le dossier **AppCore>Services>GeneralMessage**, une classe héritant de `EndPointResponse`.

    💬 *<u>Convention :</u> <br>
    => **nom de la classe**, commence par `Response`*

2. Pour chaque variable de cette classe, ajouter l'annotation `[JsonInclude]`. 

3. Puis créer cette instance dans votre EndPoint et le passer dans le paramètre **data** de `Message()`

*Exemple:*

```Csharp
var response = new ResponseGetEntireVillage(
    town,
    dbContext.ConstructionDefs.Where(x => x.VillageId == town.VillageId).ToArray(),
    dbContext.ConstructionProds.Where(x => x.VillageId == town.VillageId).ToArray(),
    dbContext.ConstructionAutres.Where(x => x.VillageId == town.VillageId).ToArray(),
    dbContext.Attaques.Where(x => x.VillageId == town.VillageId).ToArray(),
    dbContext.Coordonnees.Where(x => x.VillageId == town.VillageId).ToArray(),
    dbContext.RessourcePossedes.Where(x => x.VillageId == town.VillageId).ToArray(),
    dbContext.ObjetsPossedes.Where(x => x.VillageId == town.VillageId).ToArray()
);
return Json(Message(message.UserGuid, response));
```