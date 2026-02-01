//Constantes pour l'URL
const BASE_URL = "http://yams.iutrs.unistra.fr:3000/api/games/";

//Variables globales
let partie_id; //ID de la partie en cours
let tabJoueurs = []; //Tableau contenant les pseudos des joueurs
let scores, scoresMineurs, scoresMajeurs; //Variables pour stocker les scores*
let round_index = 1; //Indice du tour en cours
const listeChalMajeurs = ["brelan", "carre", "full", "petite", "grande", "yams", "chance"]; //Liste des challenges majeurs
const dicoChallenges = {
    // Dictionnaire des correspondances des noms de challenges
    nombre1: "Nombre de 1",
    nombre2: "Nombre de 2",
    nombre3: "Nombre de 3",
    nombre4: "Nombre de 4",
    nombre5: "Nombre de 5",
    nombre6: "Nombre de 6",
    brelan: "Brelan",
    carre: "Carré",
    full: "Full",
    petite: "Petite Suite",
    grande: "Grande Suite",
    yams: "Yams",
    chance: "Chance",
};

//Initialisation des événements
document.getElementById('partieForm').addEventListener('submit', submitPartieId); //Appelle submitPartieId quand on soumet un code de partie
document.getElementById('choixForm').addEventListener('submit', submitChoixAffichage); //Appelle submitChoixAffichage quand on soumet un mode d'affichage

//Fonction pour la récupération des paramètres de la partie après soumission du code de la partie
function submitPartieId(event) {
    event.preventDefault(); //Empêche le rechargement de la page
    partie_id = document.getElementById("partie_id").value; //Récupération du code entré

    //Récupération des paramètres de la partie
    fetch(`${BASE_URL}${partie_id}/parameters`)
        .then(response => response.json())
        .then(data => {
            afficherParametres(data);
        })
        .catch(error => console.error('Erreur : pas de partie trouvée.', error));
}

//Fonction pour afficher les paramètres de la partie
function afficherParametres(data) {
    const contenantP = document.getElementById('affichageParametres'); //Contiendra le code de groupe et la date
    const contenantF = document.getElementById('choixForm'); //Contiendra le formulaire pour soumettre le choix d'affichage

    //Remplit les paramètres de la partie
    contenantP.innerHTML = `
        <p>Code Groupe: ${data.code}</p>
        <p>Date: ${data.date}</p>
    `;

    //Crée un formulaire pour choisir le type d'affichage
    contenantF.innerHTML = `
        <form id="choixForm">
            <fieldset>
                <legend>Choisir l'affichage de la partie :</legend>
                <div class="radio-group">
                    <input type="radio" id="global" name="choix" value="global">
                    <label for="global">Affichage global</label>
                </div>
                <div class="radio-group">
                    <input type="radio" id="detail" name="choix" value="detail">
                    <label for="detail">Affichage détaillé</label>
                </div>
            </fieldset>
            <button type="submit">Valider</button>
        </form>
    `;
}

//Fonction pour la soumission du choix d'affichage : global ou détaillé
function submitChoixAffichage(event) {
    event.preventDefault(); //Empêche le rechargement de la page
    let choix = document.querySelector('input[name="choix"]:checked').value; //Récupération du choix dans le formulaire

    //Appelle la fonction correspondante au choix
    if (choix === "global") {
        affichageGlobal();
    }
    if (choix === "detail") {
        affichageDetail();
    }
}

//Fonction principale pour l'affichage global
function affichageGlobal() {
    document.getElementById("choixForm").innerHTML = ""; //On retire le formulaire de choix

    //Récupération des scores finaux et bonus
    fetch(`${BASE_URL}${partie_id}/final-result`)
        .then(response => response.json())
        .then(data => afficherScoresGlobaux(data))
        .catch(error => console.error('Erreur : pas de partie trouvée.', error));

    //Récupération des joueurs
    fetch(`${BASE_URL}${partie_id}/players`)
        .then(response => response.json())
        .then(data => {
            afficherJoueursEtScores(data);
            remplirJoueurs(data);
        })
        .catch(error => console.error('Erreur : pas de partie trouvée.', error));
}

// Prépare l'affichage des scores globaux des joueurs
function afficherScoresGlobaux(data) {
    const element = document.getElementById('affichage');
    scores = [data[0].score, data[1].score];
    scoresMineurs = [data[0].bonus, data[1].bonus];
    scoresMajeurs = [0, 0];

    // Génère les tableaux des scores
    element.innerHTML = `
        <h2>Affichage global</h2>
        <div class="joueurs"></div>
        ${genererTableauScores()}
    `;
    remplirBonusScores(data);
}

// Génère le tableau HTML des scores mineurs et majeurs
function genererTableauScores() {
    return `
            <table id="chalMineurs">
                <thead>
                    <tr><th></th><th>Joueur 1</th><th>Joueur 2</th></tr>
                </thead>
                <tbody>
                    <tr><th>1 [total de 1]</th><td></td><td></td></tr>
                    <tr><th>2 [total de 2]</th><td></td><td></td></tr>
                    <tr><th>3 [total de 3]</th><td></td><td></td></tr>
                    <tr><th>4 [total de 4]</th><td></td><td></td></tr>
                    <tr><th>5 [total de 5]</th><td></td><td></td></tr>
                    <tr><th>6 [total de 6]</th><td></td><td></td></tr>
                </tbody>
                <tfoot>
                    <tr><th>Bonus si > à 62 [35]</th><td></td><td></td></tr>
                    <tr><th>Total challenges mineurs</th><td></td><td></td></tr>
                </tfoot>
            </table>
            <table id="chalMajeurs">
                <tbody>
                    <tr><th>Brelan [total]</th><td></td><td></td></tr>
                    <tr><th>Carré [total]</th><td></td><td></td></tr>
                    <tr><th>Full [25]</th><td></td><td></td></tr>
                    <tr><th>Petite Suite [30]</th><td></td><td></td></tr>
                    <tr><th>Grande Suite [40]</th><td></td><td></td></tr>
                    <tr><th>Yams [50]</th><td></td><td></td></tr>
                    <tr><th>Chance [total]</th><td></td><td></td></tr>
                </tbody>
                <tfoot>
                    <tr><th>Total challenges majeurs</th><td></td><td></td></tr>
                </tfoot>
            </table>
            <table id="scoreTotal">
                <tbody>
                    <tr><th>Score Total</th><td></td><td></td></tr>
                </tbody>
            </table>
        `;
}

// Remplit les bonus des scores mineurs
function remplirBonusScores(data) {
    const tableauScoreMin = document.getElementById("chalMineurs");
    const tableauScoreTotal = document.getElementById("scoreTotal");

    for (let i = 0; i < 2; i++) {
        tableauScoreMin.rows[7].cells[i + 1].innerText = data[i].bonus;
        tableauScoreTotal.rows[0].cells[i + 1].innerText = data[i].score;
    }
}

// Affiche les pseudos des joueurs, leurs scores, et le gagnant
function afficherJoueursEtScores(players) {
    const parent = document.querySelector(".joueurs");

    players.forEach((player, i) => {
        const joueurElement = document.createElement("p");
        joueurElement.innerHTML = `<strong>${player.pseudo}</strong> avec un score de ${scores[i]} !`;
        parent.appendChild(joueurElement);
    });

    if (scores[0] > scores[1]) {
        const gagnantElement = document.createElement("p");
        gagnantElement.innerHTML = `Le gagnant est donc <strong>${players[0].pseudo}</strong> !`;
        parent.appendChild(gagnantElement);
    }
    else {
        const gagnantElement = document.createElement("p");
        gagnantElement.innerHTML = `Le gagnant est donc <strong>${players[1].pseudo}</strong> !`;
        parent.appendChild(gagnantElement);
    }

    remplirScoresRounds(players);
}

// Remplit les scores pour chaque tour
function remplirScoresRounds(players) {
    for (let i = 0; i < players.length; i++) {
        for (let tour = 1; tour <= 13; tour++) {
            fetch(`${BASE_URL}${partie_id}/rounds/${tour}`)
                .then(response => response.json())
                .then(data => {
                    const tableauScoreMin = document.getElementById("chalMineurs");
                    const tableauScoreMaj = document.getElementById("chalMajeurs");

                    const challenge = data.results[i].challenge;
                    const score = data.results[i].score;

                    if (/^nombre[1-6]$/.test(challenge)) {
                        const num = challenge.replace('nombre', '');
                        tableauScoreMin.rows[num].cells[i + 1].innerText = score;
                        scoresMineurs[i] += score;
                    } else {
                        const indexChal = listeChalMajeurs.indexOf(challenge);
                        tableauScoreMaj.rows[indexChal].cells[i + 1].innerText = score;
                        scoresMajeurs[i] += score;
                    }

                    tableauScoreMin.rows[8].cells[i + 1].innerText = scoresMineurs[i];
                    tableauScoreMaj.rows[7].cells[i + 1].innerText = scoresMajeurs[i];
                })
                .catch(error => console.error('Erreur : pas de partie trouvée.', error));
        }
    }
}

// Affiche les pseudos dans la première ligne du tableau
function remplirJoueurs(data) {
    const tableauScoreMin = document.getElementById("chalMineurs");

    for (let i = 0; i < 2; i++) {
        tableauScoreMin.rows[0].cells[i + 1].innerText = data[i].pseudo;
    }
}

// Fonction principale pour l'affichage détaillé
function affichageDetail() {
    round_index = 1;
    document.getElementById("choixForm").innerHTML = "";
    afficherTour(round_index);
}

// Affiche les détails d'un tour donné
function afficherTour(index) {
    if (index >= 1 && index <= 13) {

        round_index = index;

        fetch(`${BASE_URL}${partie_id}/players`)
            .then(response => response.json())
            .then(data => {
                tabJoueurs[0] = data[0].pseudo;
                tabJoueurs[1] = data[1].pseudo;
            })
            .catch(error => console.error('Erreur : pas de partie trouvée.', error));

        fetch(`${BASE_URL}${partie_id}/rounds/${index}`)
            .then(response => response.json())
            .then(data => afficherDetailsTour(data))
            .catch(error => console.error('Erreur lors du fetch :', error));
    }
    else if (index == 14) {

        round_index = index;

        fetch(`${BASE_URL}${partie_id}/players`)
            .then(response => response.json())
            .then(data => {
                tabJoueurs[0] = data[0].pseudo;
                tabJoueurs[1] = data[1].pseudo;
            })
            .catch(error => console.error('Erreur : pas de partie trouvée.', error));

        fetch(`${BASE_URL}${partie_id}/final-result`)
            .then(response => response.json())
            .then(data => afficherScoresFinaux(data))
            .catch(error => console.error('Erreur : pas de partie trouvée.', error));

    }
}

// Créer le HTML pour préparer l'affichage
function afficherDetailsTour(data) {
    const element = document.getElementById('affichage');
    element.innerHTML = `
        <div id="affichage-detail">
            <div id="arrow-left">
                <img src="resources/arrow-left.png" style="visibility: ${round_index > 1 ? 'visible' : 'hidden'};" />
            </div>
            <div id="tour">
                <h2>Affichage détaillé</h2>
                <h3 id="compte-tour">Tour n°${round_index}</h3><hr>
                <div id="tourJ1"></div><hr>
                <div id="tourJ2"></div><hr>
            </div>
            <div id="arrow-right">
                <img src="resources/arrow-right.png" />
            </div>
        </div>
    `;


    remplirDetailsTour(data);

    // Ajoute les événements pour naviguer entre les tours
    document.getElementById('arrow-left').addEventListener('click', () => afficherTour(round_index - 1));
    document.getElementById('arrow-right').addEventListener('click', () => afficherTour(round_index + 1));
}

// Affiche pour chacun des joueurs son pseudo, le challenge, son score et les dés
function remplirDetailsTour(data) {
    const compte_tour = document.getElementById('compte-tour');
    const tourJ1 = document.getElementById('tourJ1');
    const tourJ2 = document.getElementById('tourJ2');

    compte_tour.innerText = `Tour n°${round_index}`;

    [tourJ1, tourJ2].forEach((tour, i) => {
        const challenge = dicoChallenges[data.results[i].challenge];
        const score = data.results[i].score;
        const dices = data.results[i].dice;

        tour.innerHTML = `
            <div class="info-tour-group">
                <p><strong>Joueur</strong> : ${tabJoueurs[i]}</p>
                <p><strong>Challenge réalisé ce tour</strong> : ${challenge}</p>
                <p><strong>Score pour ce tour</strong> : ${score}</p>
                <p><strong>Dés</strong> :</p>
            </div>
        `;
        dices.forEach(dice => {
            tour.innerHTML += `<img src='resources/${dice}.png'>`;
        });
    });
}

// Affiche le vainqueur, le perdant, et leurs scores
function afficherScoresFinaux(data) {
    const element = document.getElementById('affichage');

    let message;
    switch (true) {
        case data[0].score > data[1].score:
            message = `Le vainqueur est <strong>${tabJoueurs[0]}</strong> avec un score final de ${data[0].score} points !<br>
        <strong>${tabJoueurs[1]}</strong> perd avec un score final de ${data[1].score} points.`;
            break;
        case data[0].score < data[1].score:
            message = `Le vainqueur est <strong>${tabJoueurs[1]}</strong> avec un score final de ${data[1].score} points !<br>
        <strong>${tabJoueurs[1]}</strong> perd avec un score final de ${data[1].score} points.`;
            break;
        default:
            message = `Égalité ! Les deux joueurs ont marqué ${data[0].score} points !`;
    }

    element.innerHTML = `
        <div id="affichage-detail">
            <div id="arrow-left">
                <img src="resources/arrow-left.png"/>
            </div>
            <div id="resultats">
                <p>${message}</p>
                <img src="resources/trophy.png" width="20%" height="20%"/>
            </div>
            <div id="arrow-right">
                <img src="resources/arrow-right.png" style="visibility: hidden;"/>
            </div>
        </div>
    `;

    document.getElementById('arrow-left').addEventListener('click', () => afficherTour(round_index - 1));
}
