const API_BASE_URL = "https://localhost:64984";

// Constante qui = WorldCOnfig dans API
const WORLD_WIDTH = 50;
const WORLD_HEIGHT = 50;

// arte affichée (coin haut-gauche + taille)
const SQUARE_VIEW = 5;
let MiddleXView = 10;
let MiddleYView = 10;

//Fct au démarrage (loading)
$(function () {
  let $startTuile;

  majInfosStatiques();

  for (let y = MiddleYView; y < MiddleYView + SQUARE_VIEW; y++) {
    for (let x = MiddleXView; x < MiddleXView + SQUARE_VIEW; x++) {
      const $tuile = creerTuile(x, y);
      if (x === MiddleXView && y === MiddleYView) {
        $startTuile = $tuile;
      }
    }
  }

  chargerTuile(MiddleXView, MiddleYView, $startTuile).done(function () {
    const neighbors = getNeighboringTiles(
      MiddleXView,
      MiddleYView,
      WORLD_WIDTH,
      WORLD_HEIGHT,
    );
    for (const n of neighbors) {
      if (
        n.x < MiddleXView ||
        n.x >= MiddleXView + SQUARE_VIEW ||
        n.y < MiddleYView ||
        n.y >= MiddleYView + SQUARE_VIEW
      )
        continue;

      const $neighborTuile = creerTuile(n.x, n.y);
      chargerTuile(n.x, n.y, $neighborTuile);
    }
  });
});
$("#map-grid").on("click", ".tile", function () {
  const $tuile = $(this);
  const x = $tuile.data("x");
  const y = $tuile.data("y");

  if ($tuile.hasClass("tile-loaded")) {
    afficherSelection($tuile.data("tuileInfo"), x, y);
  } else {
    chargerTuile(x, y, $tuile).done(function (data) {
      afficherSelection(data, x, y);
    });
  }
});

function majInfosStatiques() {
  $("#info-taille-value").text(`${WORLD_WIDTH} × ${WORLD_HEIGHT}`);
  $("#info-zone-value").text(`${SQUARE_VIEW}x${SQUARE_VIEW} tuiles`);
  $("#zone-affiche").html(
    `<strong>Zone: (${MiddleXView},${MiddleYView})➡️(${MiddleXView + SQUARE_VIEW - 1},${MiddleYView + SQUARE_VIEW - 1})</strong>`,
  );
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

function afficherSelection(data, x, y) {
  $("#posTop").text(`(${x}, ${y})`);
  $("#tuile-pos-value").text(`(${x}, ${y})`);
  $("#tuile-type-value").text(data.typeTxt);
  $("#tuile-passable-value").text(data.isTraversable ? "✅" : "❌");
  $("#tuile-desc-value").text(
    data.description || `Tuile ${data.typeTxt} en position (${x},${y})`,
  );
}

function creerTuile(x, y) {
  const $existing = $(`.tile[data-x="${x}"][data-y="${y}"]`);
  if ($existing.length) return $existing;

  let $tuile = $("<button> </button>");
  $tuile.addClass("tile tile-unloaded");
  $tuile.attr("data-x", x);
  $tuile.attr("data-y", y);
  $tuile.text("?");
  $tuile.appendTo("#map-grid");
  return $tuile;
}

function chargerTuile(x, y, $tuile) {
  return $.getJSON(`${API_BASE_URL}/api/Tile/${x}/${y}`)
    .done(function (data) {
      $tuile.data("tuileInfo", data);
      $tuile.removeClass("tile-unloaded").addClass("tile-loaded");
      majStatutApi(true);
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
    })
    .fail(function (xhr, textStatus, error) {
      // 404 = hors du monde, l'API répond quand même
      majStatutApi(xhr.status !== 0);
      console.error(
        `[Grid Error] Could not load position (${x}, ${y}):`,
        error,
      );
    });
}

function getNeighboringTiles(x, y, worldWidth, worldHeight) {
  const neighbors = [];

  for (let dx = -1; dx <= 1; dx++) {
    for (let dy = -1; dy <= 1; dy++) {
      // loop over center
      if (dx === 0 && dy === 0) continue;

      const nX = x + dx;
      const nY = y + dy;

      // verify if neighbor is in boundaries
      if (nX >= 0 && nX < worldWidth && nY >= 0 && nY < worldHeight) {
        neighbors.push({ x: nX, y: nY });
      }
    }
  }

  return neighbors;
}
