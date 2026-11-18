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

  appelApi(
    "POST",
    "/api/Auth/register",
    {
      email: email,
      motDePasse: mdp,
      pseudo: $("#register-pseudo").val(),
    },
    $("#auth-message"),
  ).done(function () {
    // connecte direct après l'inscription
    connexion(email, mdp);
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
  appelApi(
    "POST",
    "/api/Auth/login",
    { email: email, motDePasse: mdp },
    $("#auth-message"),
  ).done(function (data) {
    localStorage.setItem("token", data.token);
    chargerPersonnage();
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

function chargerPersonnage($msgObj = $("#auth-message")) {
  appelApi("GET", "/api/Personnage/self", null, $msgObj).done(function (data) {
    afficherJeu();
    majJeu(data);
  });
}

$("#btn-nord").on("click", () => deplacer("N"));
$("#btn-sud").on("click", () => deplacer("S"));
$("#btn-est").on("click", () => deplacer("E"));
$("#btn-ouest").on("click", () => deplacer("O"));
$("#btn-actualiser").on("click", () => chargerPersonnage($("#deplacement-message")));

function deplacer(direction) {
  appelApi(
    "POST",
    "/api/Personnage/deplacer",
    { direction: direction },
    $("#deplacement-message"),
  ).done(function (data) {
    majJeu(data);
  });
}

// reponse = { personnage, tuiles } (meme réponse pour self et deplacer)
function majJeu(reponse) {
  personnage = reponse.personnage;

  afficherStats(personnage);
  dessinerCarte(reponse.tuiles);
  majTuilesChargees();
  majZone();
}

// juste les stats du perso dans le menu de gauche
function afficherStats(perso) {
  $("#perso-nom").text(perso.nom);
  $("#perso-niveau").text(perso.niveau);
  $("#perso-xp").text(perso.experience);
  $("#perso-pv").text(`${perso.pointsVie} / ${perso.pointsVieMax}`);
  $("#perso-force").text(perso.force);
  $("#perso-defense").text(perso.defense);
  $("#perso-pos").text(`(${perso.positionX}, ${perso.positionY})`);
}

// #endregion

// #region Carte

function dessinerCarte(tuiles) {
  $("#map-grid").empty();

  for (
    let y = personnage.positionY - VISION;
    y <= personnage.positionY + VISION;
    y++
  ) {
    for (
      let x = personnage.positionX - VISION;
      x <= personnage.positionX + VISION;
      x++
    ) {
      const $tuile = creerTuile(x, y);

      if (x === personnage.positionX && y === personnage.positionY) {
        $tuile.addClass("tile-perso");
      }

      const infoTuile = tuiles.find((t) => t.x === x && t.y === y);
      if (infoTuile) {
        afficherTuile($tuile, infoTuile);
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
  appelApi(
    "POST",
    `/api/Personnage/explorer/${x}/${y}`,
    null,
    $("#deplacement-message"),
  ).done(function (infoTuile) {
    afficherTuile($tuile, infoTuile);
    afficherSelection(infoTuile, x, y);
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

function afficherTuile($tuile, infoTuile) {
  $tuile.data("tuileInfo", infoTuile);
  $tuile.removeClass("tile-unloaded").addClass("tile-loaded");

  if (infoTuile.isTraversable === false) {
    $tuile.addClass("tile-bloquee");
  }

  if (infoTuile.imgUrl) {
    const $img = $("<img>", {
      src: API_BASE_URL + infoTuile.imgUrl,
      alt: `${infoTuile.description} ${infoTuile.typeTxt}`,
      class: "tile-img",
    });
    $tuile.empty().append($img);
  } else {
    //fallback img not working
    $tuile.text(infoTuile.typeTxt);
  }
}

function afficherSelection(infoTuile, x, y) {
  $("#tuile-pos-value").text(`(${x}, ${y})`);
  $("#tuile-type-value").text(infoTuile.typeTxt);
  $("#tuile-passable-value").text(infoTuile.isTraversable ? "✅" : "❌");
  $("#tuile-desc-value").text(
    infoTuile.description ||
      `Tuile ${infoTuile.typeTxt} en position (${x},${y})`,
  );
}

// #endregion

// #region Infos

function majInfosStatiques() {
  $("#info-taille-value").text(`${WORLD_WIDTH} × ${WORLD_HEIGHT}`);
  $("#info-zone-value").text(`${SQUARE_VIEW}x${SQUARE_VIEW} tuiles`);
}

// update zone display around personnage
function majZone() {
  if (!personnage) return;

  $("#zone-affiche").html(
    `<strong>Zone: (${personnage.positionX - VISION},${personnage.positionY - VISION})➡️(${personnage.positionX + VISION},${personnage.positionY + VISION})</strong>`,
  );
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
function appelApi(method, url, body, $msgObj) {
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
      if ($msgObj) {
        $msgObj.text("");
      }
    })
    .fail(function (xhr) {
      majStatutApi(xhr.status !== 0);

      // token expiré ou invalide = retour à la connexion
      if (xhr.status === 401 && token) {
        $("#auth-message").text(
          "Votre connexion a expirée, veuillez vous reconnecter.",
        );
        localStorage.removeItem("token");
        afficherConnexion();
      }

      if ($msgObj) {
        $msgObj.text(messageErreur(xhr));
      }
    });
}

function messageErreur(xhr) {
  if (xhr.responseJSON && xhr.responseJSON.message)
    return xhr.responseJSON.message;

  // erreurs de validation du model ([Required], [MinLength], ...)
  if (xhr.responseJSON && xhr.responseJSON.errors) {
    return Object.values(xhr.responseJSON.errors).flat().join(" ");
  }

  if (xhr.status === 0) return "API inaccessible.";
  return "Erreur " + xhr.status;
}

// #endregion
