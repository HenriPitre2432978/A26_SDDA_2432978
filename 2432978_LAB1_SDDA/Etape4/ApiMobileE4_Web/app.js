const API_BASE_URL = "https://localhost:64984";

// Constante qui = WorldCOnfig dans API
const WORLD_WIDTH = 50;
const WORLD_HEIGHT = 50;

// carte affichée 5x5, centrée sur le perso
const SQUARE_VIEW = 5;
const VISION = 2;

let personnage = null;

//Fct au démarrage (loading)
$(function () {
  majInfosStatiques();

  // déja connecté avant (token gardé dans le navigateur)
  if (localStorage.getItem("token")) {
    chargerPersonnage();
  } else {
    afficherConnexion();
  }
});

// #region Connexion / Inscription

$("#form-login").on("submit", function (e) {
  e.preventDefault();

  connexion($("#login-email").val(), $("#login-mdp").val());
});

$("#form-register").on("submit", function (e) {
  e.preventDefault();

  const email = $("#register-email").val();
  const mdp = $("#register-mdp").val();

  appelApi("POST", "/api/Auth/register", {
    email: email,
    motDePasse: mdp,
    pseudo: $("#register-pseudo").val(),
  })
    .done(function () {
      // connecte direct après l'inscription
      connexion(email, mdp);
    })
    .fail(function (xhr) {
      $("#auth-message").text(messageErreur(xhr));
    });
});

$("#btn-logout").on("click", function () {
  appelApi("POST", "/api/Auth/logout").always(function () {
    // le serveur garde pas le token, c'est le client qui l'oublie
    localStorage.removeItem("token");
    personnage = null;
    afficherConnexion();
  });
});

function connexion(email, mdp) {
  appelApi("POST", "/api/Auth/login", { email: email, motDePasse: mdp })
    .done(function (data) {
      localStorage.setItem("token", data.token);
      $("#auth-message").text("");
      chargerPersonnage();
    })
    .fail(function (xhr) {
      $("#auth-message").text(messageErreur(xhr));
    });
}

function afficherConnexion() {
  $("#jeu").hide();
  $("#btn-logout").hide();
  $("#auth-section").show();
}

function afficherJeu() {
  $("#auth-section").hide();
  $("#jeu").show();
  $("#btn-logout").show();
}

// #endregion

// #region Personnage / Déplacement

function chargerPersonnage() {
  appelApi("GET", "/api/Personnage/self").done(function (data) {
    afficherJeu();
    majJeu(data);
  });
}

$("#btn-nord").on("click", () => deplacer("N"));
$("#btn-sud").on("click", () => deplacer("S"));
$("#btn-est").on("click", () => deplacer("E"));
$("#btn-ouest").on("click", () => deplacer("O"));
$("#btn-actualiser").on("click", () => chargerPersonnage());

function deplacer(direction) {
  appelApi("POST", "/api/Personnage/deplacer", { direction: direction })
    .done(function (data) {
      $("#deplacement-message").text("");
      majJeu(data);
    })
    .fail(function (xhr) {
      // ex: eau, montagne, bord du monde
      $("#deplacement-message").text(messageErreur(xhr));
    });
}

// data = { personnage, tuiles } (meme réponse pour self et deplacer)
function majJeu(data) {
  personnage = data.personnage;

  $("#perso-nom").text(personnage.nom);
  $("#perso-niveau").text(personnage.niveau);
  $("#perso-xp").text(personnage.experience);
  $("#perso-pv").text(`${personnage.pointsVie} / ${personnage.pointsVieMax}`);
  $("#perso-force").text(personnage.force);
  $("#perso-defense").text(personnage.defense);
  $("#perso-pos").text(`(${personnage.positionX}, ${personnage.positionY})`);
  $("#posTop").text(`(${personnage.positionX}, ${personnage.positionY})`);

  dessinerCarte(data.tuiles);
  majInfosStatiques();
}

// #endregion

// #region Carte

function dessinerCarte(tuiles) {
  $("#map-grid").empty();

  for (let y = personnage.positionY - VISION; y <= personnage.positionY + VISION; y++) {
    for (let x = personnage.positionX - VISION; x <= personnage.positionX + VISION; x++) {
      const $tuile = creerTuile(x, y);

      if (x === personnage.positionX && y === personnage.positionY) {
        $tuile.addClass("tile-perso");
      }

      const data = tuiles.find((t) => t.x === x && t.y === y);
      if (data) {
        afficherTuile($tuile, data);
      }
    }
  }
}

$("#map-grid").on("click", ".tile", function () {
  const $tuile = $(this);
  const x = $tuile.data("x");
  const y = $tuile.data("y");

  if ($tuile.hasClass("tile-vide")) return;

  if ($tuile.hasClass("tile-loaded")) {
    afficherSelection($tuile.data("tuileInfo"), x, y);
  } else {
    explorerTuile(x, y, $tuile);
  }
});

function explorerTuile(x, y, $tuile) {
  appelApi("POST", `/api/Personnage/explorer/${x}/${y}`)
    .done(function (data) {
      afficherTuile($tuile, data);
      afficherSelection(data, x, y);
    })
    .fail(function (xhr) {
      $("#deplacement-message").text(messageErreur(xhr));
    });
}

function creerTuile(x, y) {
  let $tuile = $("<button> </button>");
  $tuile.addClass("tile tile-unloaded");
  $tuile.attr("data-x", x);
  $tuile.attr("data-y", y);
  $tuile.text("?");

  // case hors du monde
  if (x < 0 || x >= WORLD_WIDTH || y < 0 || y >= WORLD_HEIGHT) {
    $tuile.addClass("tile-vide").text("");
  }

  $tuile.appendTo("#map-grid");
  return $tuile;
}

function afficherTuile($tuile, data) {
  $tuile.data("tuileInfo", data);
  $tuile.removeClass("tile-unloaded").addClass("tile-loaded");
  majTuilesChargees();

  if (data.imgUrl) {
    const $img = $("<img>", {
      src: API_BASE_URL + data.imgUrl,
      alt: `${data.description} ${data.typeTxt}`,
      class: "tile-img",
    });
    $tuile.empty().append($img);
  } else {
    //fallback img not working
    $tuile.text(data.typeTxt);
  }
}

function afficherSelection(data, x, y) {
  $("#tuile-pos-value").text(`(${x}, ${y})`);
  $("#tuile-type-value").text(data.typeTxt);
  $("#tuile-passable-value").text(data.isTraversable ? "✅" : "❌");
  $("#tuile-desc-value").text(
    data.description || `Tuile ${data.typeTxt} en position (${x},${y})`,
  );
}

// #endregion

// #region Infos

function majInfosStatiques() {
  $("#info-taille-value").text(`${WORLD_WIDTH} × ${WORLD_HEIGHT}`);
  $("#info-zone-value").text(`${SQUARE_VIEW}x${SQUARE_VIEW} tuiles`);

  if (personnage) {
    $("#zone-affiche").html(
      `<strong>Zone: (${personnage.positionX - VISION},${personnage.positionY - VISION})➡️(${personnage.positionX + VISION},${personnage.positionY + VISION})</strong>`,
    );
  }
  majTuilesChargees();
}

function majTuilesChargees() {
  $("#info-chargees-value").text(
    `${$(".tile-loaded").length} / ${SQUARE_VIEW * SQUARE_VIEW}`,
  );
}

function majStatutApi(ok) {
  $("#info-status-value")
    .text(ok ? "Connecté" : "Hors ligne")
    .css("color", ok ? "limegreen" : "red");
}

// #endregion

// #region Appels API

// tous les appels passent ici pour ajouter le token
function appelApi(method, url, body) {
  const token = localStorage.getItem("token");

  return $.ajax({
    method: method,
    url: API_BASE_URL + url,
    contentType: "application/json",
    data: body ? JSON.stringify(body) : undefined,
    headers: token ? { Authorization: `Bearer ${token}` } : {},
  })
    .done(function () {
      majStatutApi(true);
    })
    .fail(function (xhr) {
      majStatutApi(xhr.status !== 0);

      // token expiré ou invalide = retour à la connexion
      if (xhr.status === 401 && token) {
        localStorage.removeItem("token");
        afficherConnexion();
      }
    });
}

function messageErreur(xhr) {
  if (xhr.responseJSON && xhr.responseJSON.message) return xhr.responseJSON.message;

  // erreurs de validation du model ([Required], [MinLength], ...)
  if (xhr.responseJSON && xhr.responseJSON.errors) {
    return Object.values(xhr.responseJSON.errors).flat().join(" ");
  }

  if (xhr.status === 0) return "API inaccessible.";
  return "Erreur " + xhr.status;
}

// #endregion
