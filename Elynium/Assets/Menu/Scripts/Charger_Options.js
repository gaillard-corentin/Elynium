var texte;
texte = GetComponentInChildren(MeshRenderer);

function OnMouseEnter() {
    texte.material.color = Color.red;
}

function OnMouseExit() {
    texte.material.color = Color.white;
}

function OnMouseUp() {
    Application.LoadLevel("Options");
}

function OnMouseDown() {
    texte.material.color = Color.green;
}