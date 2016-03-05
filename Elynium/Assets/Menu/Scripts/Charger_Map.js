var texte;
texte = GetComponentInChildren(MeshRenderer);
var Map_to_load;

function OnMouseEnter() {
    texte.material.color = Color.red;
}

function OnMouseExit() {
    texte.material.color = Color.white;
}

function OnMouseUp() {
    Application.LoadLevel(Map_to_load);
}

function OnMouseDown() {
    texte.material.color = Color.green;
}

function SelectMap_Les_plages_dElynium() {
    Map_to_load = "ElyniumTestMap";
}