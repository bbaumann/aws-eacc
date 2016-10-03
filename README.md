# AWS-eacc
"Game Master" software for the eacc project : a race where cars are amazon ec2 instances and engines/weapons/pilot are other AWS related services (S3, dynamodb, sqs). These cars interacts with this software to race.

This project was done for Eurosport IT Dept, as a fun way to practice with AWS, the real game took place in a "hackathon" between teams of 8 people. The source code contains the GameMaster Tooling (DB, WS for registering a car, Console to make an event happens etc.). This code is dirty : this is disposable code done to work and not to be maintained. But it works and it may help some other folks so I share it.

The rest of the Readme is the rule book (in French)

## Rôle du Maitre de la Course
* Il possède à chaque instant la vue totale de la course, incluant :
** La liste des bolides de chaque écurie, chaque nouveau bolide entrant en course devant obligatoirement être enregistré auprès du Maitre de la Course avant de commencer à progresser dans le jeu.
** Les caractéristiques de chaque bolides (Nb Modules de pilotage, Moteur et Arme)
** Le nombre de Points de victoire accumulés
** Le budget restant pour chacune des équipes
* Il gère l’anneau de course, et le positionnement et effet des « Bonus »
* Il gère le déroulement de chacun des tours de course :
** Déclaration de l’événement pour le prochain tour
** Résolution des combats
** Contrôle des changements de caractéristique des bolides (Evolution du Nb de modules):
*** Au sein d’une même écurie
*** Entre écuries suite à un combat.
*** Retrait de la course des bolides arrêtés
* Il se réserve le droit de pouvoir interroger chacun des bolides tout au long de la course. Toute réponse en erreur ou valeur de retour fausse entrainera une pénalité.

## Objectif du Challenge
Chaque équipe joue le rôle d’une écurie. Cette écurie va créer des véhicules puis les engager dans une course style « Fous du volants » sur une piste en anneau de 300 cases. Durant la course, chaque écurie gagne des points de victoire. A la fin du jeu, l’équipe ayant amassée le plus de points de victoire gagne.

## Déroulement de la course
La course démarre par une phase de préparation et d’enregistrement des voitures. Ensuite, le départ de la course est donné. La course se déroulera au « tour par tour ». En parallèle de ces tours, une équipe avec assez de budget peut préparer un nouveau bolide, il débutera au tour suivant son enregistrement

### Créer un véhicule
Un véhicule est divisé en « modules ». Lors de son inscription, chaque véhicule peut contenir jusqu’à 10 modules. Cette limite pourra être dépassée durant la course. Il existe trois types modules différents :
# Module de pilotage
Si un véhicule ne comporte plus aucun module de pilotage, il n’a plus de pilote, il est abandonné et immédiatement retiré du jeu.
# Module Moteur
permet d’avancer lors des phases de course. Plus le véhicule comporte de moteurs, plus il avance vite. Durant une étape « En piste », si un véhicule n’a plus de moteur il s’arrête et est retiré du jeu.
# Module Arme
permet d’attaquer les véhicules des autres écuries et les véhicules pirates. Plus un véhicule contient d’armes, plus il sera avantagé dans le combat. Durant une phase de combat, si un véhicule n’a pas d’arme, il est détruit par l’adversaire et il est retiré du jeu.
Chaque véhicule voulant participer à la course doit s’enregistrer auprès de l’organisateur, déclarer ses modules et payer les frais d’inscriptions.

### Créer un véhicule techniquement
Un véhicule est une instance EC2 dans le monde AWS.
L’instance EC2 écoute sur une adresse http et doit répondre à différents appels (les spécs techniques précises seront données par ailleurs):
* http://domain/engine
Cet appel renvoie le nom d’un bucket S3. Chaque fichier présent à la racine du bucket représente un module moteur.
* http://domain/pilot
Cet appel renvoie le nom d’une queue SQS. Cette pile doit contenir un message visible dont le Body contient le nombre de modules de pilotage du véhicule.
* http://domain/weapon
Cet appel renvoie le nom d’une table DynamoDB. Chaque ligne dans la table représente un module arme (id,name).
* http://domain/state
Renvoie l’état du véhicule : son nom d’instance, le nombre et le type de ces modules ainsi que la case sur laquelle il se trouve.

Une fois les modules créés et le véhicule instancié, il faut l’enregistrer auprès du MC. Le MC fournira un endpoint qui prendra en entrée le nom de l’instance. Le endpoint répondra s’il a effectivement enregistré le véhicule ou non. Le bucket, la queue et la table DynamoDB doivent être à jour par rapport à l’état du véhicule. Par exemple, si un véhicule perd un module moteur, un fichier devra être supprimé de son bucket.

### Phase de course
La phase de course est divisée en tours de jeu.
Au début de chaque tour les véhicules avancent, c’est l’étape « en piste ». Ensuite, pour chaque véhicule, un événement aléatoire peut avoir lieu

* « En piste » (événement commun à tous les véhicules): les véhicules doivent alors enclencher leurs moteurs et avancer sur le plateau de jeu. Pour chaque moteur présent dans le véhicule, celui-ci avance d’une case. Si un véhicule n’a plus de moteur, il est retiré du jeu. A la fin de « En piste », si deux véhicules se trouvent assez proches alors un combat peut avoir lieu.

* Pirates ! : des pirates attaquent le véhicule, un combat s’ensuit.
* Casse : Une casse est en vue. Le véhicule peut tenter d’aller récupérer un module. Si le véhicule choisit d’explorer la casse, il ne joue pas le prochain tour. L’exploration peut permettre de gagner des modules. Des événements inattendus peuvent aussi se produire lors de la fouille d’une casse.
* Zone minée : le véhicule peut choisir de traverser le champ de mines ou de le contourner. Si un véhicule traverse un zone minée il peut être endommagé et perdre un composant. S’il choisit de le contourner, il perd du temps et ne jouera pas les 3 prochains tours.
* Garagiste : Le véhicule peut s’arrêter chez le garagiste. Dans le garage, le véhicule peut être réparé : l’écurie peut acheter et assembler un module sur le véhicule. Le véhicule perd 1 case s’il choisit d’acheter un module. 


### Phase de course technique

* En piste :
Le software du MC fait une requête à chaque véhicule pour connaitre son nombre de moteurs.
GET http://domain/engine/count
Si un combat est possible le software du MC en informe les deux véhicules (voir Annexe 2 :Combat)
Le software annonce ensuite sa nouvelle position au véhicule avec une nouvelle requête
PATCH http://domain/state {«square» : 42}
* Pirates :
Le software du MC fait une requête au véhicule combattant pour connaitre l’url de son algorithme de combat (spécs par ailleurs). 
GET http://domain/fight/iaurl 
S’en suit la phase de combat.
* Casse 
Le software du MC fait une requête au véhicule pour savoir s’il veut explorer la casse :
GET http://domain/event/wreck 
Si le véhicule veut s’arrêter, le software du MC fait une requête pour lui indiquer le résultat de la fouille
POST http://domain/weapon|engine|pilot {« name » : « lenomdumodule »}
ou un autre événement.
Le véhicule doit alors faire les changements dans le module S3/la queue SQS ou la table dynamodb et répondre à l’appel. La limite de 10 modules ne s’applique pas.
* Zone minée 
Le software du MC fait une requête au véhicule pour savoir s’il veut traverser la zone minée :
GET http://domain/event/mine
En cas de réponse positive, le software du MC fait potentiellement une requête au véhicule pour lui signifier la perte d’un composant
DELETE http://domain/engine/name ou DELETE http://domain/weapon/name ou DELETE http://domain/pilot/filename
* Garagiste
Le software du MC fait une requête au véhicule pour lui indiquer les prix et savoir s’il veut acheter quelque chose.
POST http://domain/event/garage {“engine”:X, pilot:”Y”,”weapon”:”Z”}
Si le véhicule veut acheter un module, il l’indique dans sa réponse. Le software du MC vérifiera alors le budget et notifiera l’ajout du module :
POST http://domain/weapon|engine|pilot {« name » : « lenomdumodule »}
Le véhicule doit alors faire les changements dans le module S3/la queue SQS ou la table dynamodb et répondre à l’appel. La limite de 10 modules ne s’applique pas

A chaque changement de position, le software du MC peut faire un appel à un vaisseau pour lui indiquer sa nouvelle position
PUT http://domain/state/position
A tout moment, le software du MC peut faire un appel à un vaisseau pour lui demander son état
* GET http://domain/state
Après la plupart des modification d’un véhicule (avancée sur la piste, perte d’un module, gain d’un module…), le software du MC ira vérifier que la modification est bien connue du véhicule. En cas d’erreur, le véhicule subit une pénalité : il n’avancera pas lors de la prochaine étape « en piste ». En cas de pénalité, le software du MC fera une requête pour renvoyer la bonne valeur.

Les informations envoyées par le Maitre de la Course font foi. Toute réclamation doit être envoyée au Jury de Course (eacc-jurycourse@discoverycomm.onmicrosoft.com) et sera traité, soit durant la course par le Jury, soit prise en compte sur le résultat final.

### Combat
A la fin d’un déplacement en phase « en piste » deux véhicules peuvent se retrouver assez proches. Dans ce cas ils peuvent se combattre. Les combats concernent toujours deux véhicules. Si plus de deux véhicules sont présents sur une case, l’algorithme du MC décidera de l’ordre de résolution. Un combat peut aussi avoir lieu avec des pirates. Le combat entre se déroule de la manière suivante :
1.	MC demande aux véhicules s’ils veulent combattre
GET http://domain/fight/shouldfight?id=otherTeamId 
Si aucun des deux véhicules ne veut combattre, le combat n’a pas lieu.
Si au moins un véhicule veut combattre, le combat commence. Si un véhicule n’a pas de module d’arme, il est détruit et retiré du jeu. Si le combat a lieu, le MC demande aux véhicules l’adresse de leur algorithme de combat, celui-ci est une AWS Lambda :
GET http://domain/fight/iaurl
2.	Les règles du combat sont décrites en annexe 
3.	Le véhicule vaincu perd un module au hasard et ne jouera pas la prochaine étape « en piste ».
PATCH http://domain/state pour la nouvelle position
DELETE http://domain/engine/name ou DELETE http://domain/weapon/name ou DELETE http://domain/pilot/filename 
S’en suivra une phase de vérification.

## Budget
Chaque équipe démarre avec un budget fixe qui lui permet d’affréter des véhicules et de les réparer dans les garages. Au cours du jeu, des budgets supplémentaires seront attribués aux équipes.
Budget initial pour chacune des écuries : 100$
Coût d’un module Pilote : 1$
Coût d’un module Moteur : 2$
Coût d’un module Arme : 2$

A tout moment, une équipe peut interroger le WS du MC pour savoir leur budget restant.
A propos des projets réalisés
* Chaque équipe décide du niveau de développement / automatisation des codes et scripts déployés pour la course, ex :
** Il est possible d’automatiser la gestion des VM, ou de réaliser les modifications manuellement (mais dans les temps impartis !).
** Chaque équipe décidera du niveau de perfectionnement de leur console de management du Jeu.
* La réalisation minimum étant les différents composants nécessaires au déroulement du jeu, composants, qui si inexistants, ou défaillants, entraineront des pénalités.
* Les équipes ont toute latitude pour faire évoluer/optimiser le code de leurs composants durant la course. Tout composant inopérant, ou buggé impliquant potentiellement des pénalités.
* Le MC fournira les accès aux Logs de jeu de leur écurie, afin que les équipes puissent les exploiter comme bon leur semble (affichage sur une console d’administration, contrôle du déroulement des phases de Jeu (Ex combats))

## Points de victoire
Au cours du jeu, chaque équipe voit son nombre de points de victoire, calculé selon la distance cumulée parcourue par chaque véhicule jusqu’à son éventuel mise hors course, ou fin de course:
* 30°case atteinte : 1PV
* 50°case atteinte : 2PV
* 100° case atteinte : 8PV
* 200° case atteinte : 18PV
* 300° case atteinte : 30PV
* Au-delà : 15PV toutes les 100 cases

Par exemple, un véhicule détruit sur la case 156 rapporte 8PV, un véhicule détruit sur la case 98 rapporte lui 2PV.
Le Maitre de la Course calculera le nombre de PV à l’issue de chaque tour.
En plus de ces points de victoires, des PV « bonus » sont accordés à la fin du jeu pour l’écurie qui aura plus performé que les autres dans les domaines suivants :

* 20PV pour l’écurie propriétaire du véhicule qui aura gagné le plus de combats
* 20PV pour l’écurie qui aura gagné le plus de combat
* 20PV pour l’écurie propriétaire du véhicule qui aura parcouru la plus grande distance.
* 20PV pour l’écurie qui aura parcouru la plus grande distance cumulée.
Au cours de la partie, des achievements font aussi gagner des PV. Certains de ces achievements sont connus de tous, d’autres sont cachés.


## Fin du jeu
Le jeu se termine au bout de 2h de jeu. L’équipe ayant le plus de PV est déclarée vainqueur.


## Achievements
Durant la course, vous pouvez gagner des achievements. Ces faits de jeu sont récompensés par une médaille, et éventuellement des eCC et des PV. Les bonus de ces achievements ne sont valables qu’une seule fois
Tous les achievements sont initialement cachés, des indices seront données en cours de jeu.

## Annexe : Règles du jeu de combat

### But du jeu

Le combat s’apparente à un duel (un « chou fleur » en plus compliqué). Les joueurs sont face à face sur une piste de combat et vont tour à tour avancer en jouant des cartes. Le but du jeu est d’arriver sur la case du joueur adverse.
Ce jeu est inspiré du jeu de société « En Garde ».

### Début du combat
Un joueur démarre sur la case 1. L’autre joueur démarre sur la case 23.
Chaque joueur reçoit autant de cartes que son véhicule a d’armes.
Le premier joueur est déterminé au hasard.
Il y a 25 cartes dans la pioche : cinq « 1 », cinq « 2 », cinq « 3 », cinq « 4 » et cinq « 5 »

### Déroulement d’un tour de combat
Le premier joueur prend connaissance des coups précédents et de ses cartes. Il choisit alors entre se déplacer et attaquer.
Se déplacer
Le joueur joue une carte, dont la valeur va de 1 à 5 et avance d’autant de cases sur la piste de jeu.
Un joueur ne peut pas « dépasser » son adversaire. 

Ex : Le joueur blanc vient de jouer un « 3 », il passe de la case 1 à la case 4.

#### Attaquer directement
Si les deux joueurs sont assez proches, le joueur actif peut attaquer. Dans ce cas, il joue une ou plusieurs cartes dont la valeur est égale à la distance qui sépare les deux joueurs.
Le joueur adverse doit parer l’attaque : il doit jouer autant de cartes de la même valeur que celles de l’attaquant :
* S’il ne peut pas le faire, le joueur est touché, il perd le combat.
* S’il réussit à parer, le joueur attaqué ne pioche pas de cartes et c’est alors à lui de jouer son tour normalement.
Cas particulier : le défenseur pare mais n’a plus de cartes. Le défenseur ne peut pas jouer son prochain tour, il a alors perdu. 
 
Ex : Le joueur noir joue deux « 4 » et attaque le joueur blanc. Si le joueur blanc a deux « 4 » il les défausse et joue son tour de jeu. S’il ne les a pas il perd le combat.

#### Attaquer indirectement
Le joueur actif peut jouer à la fois un déplacement et une attaque. Le déplacement et l’attaque suivent les règles énoncées ci-dessus. 
 
Ex : Le joueur blanc, initialement présent en 9 a beaucoup de chance, il joue un un et trois « 4 », il se déplace en 10 et attaque le joueur noir. Comme il n’y a que 5 « 4 » en tout, le joueur ne peut pas parer. Blanc gagne.

Cas particulier : si un joueur ne peut jouer aucun coup valide (par exemple il n’a qu’un 5 et il est à 2 cases de distance de l’adversaire), il a perdu. 
Cas particulier : si la pioche est épuisée et qu’un joueur doit piocher, il y a match nul. Les deux véhicules repartent sans dommage.

### Fin du tour de jeu
A la fin de son tour de jeu, le joueur pioche pour compléter sa main jusqu’à son nombre de cartes initiales. C’est alors à l’adversaire de jouer.

## Spécs techniques
En pratique, chaque équipe devra créer une IA de combat sous la forme d’une AWS Lambda déclenchée par une API Gateway.
L’url de l’API devra être communiquée au MC.
L’IA répondra à un seul type appel. Cet appel contiendra la liste des cartes en main, la liste des coups précédents du joueur ainsi que ceux de son adversaire.
L’IA devra répondre un objet JSON qui contiendra le coup à jouer. 
Exemple d’appel :
GET https://XXXXXXXX.execute-api.eu-central-1.amazonaws.com/prod/action?yourmoves=[1,5,4,1]&othermoves=[1,2,2,4]&cards=[3,2,1,1,2,2]

### Réponses possibles
{
  "res":[1]
}
=> Le joueur avance/attaque d’une case
{
  "res":[2]
}
=> Le joueur avance/attaque de deux cases
{
  "res":[2,2]
}
=> Le joueur attaque avec deux cartes
{
  "res":[2,2,2]
}
=> Le joueur attaque avec trois cartes
{
  "res":[1,2]
}
=> Le joueur fait attaque indirecte. Il avance d’une case et attaque à deux cases
{
  "res":[1,2,2]
}
=> Le joueur fait attaque indirecte. Il avance d’une case et attaque à deux cases avec deux cartes
{
  "res":[2,2]
}
=> Le joueur fait attaque indirecte. Il avance de deux cases et attaque à deux cases

Toute autre réponse est un coup illégal et perd la partie.
