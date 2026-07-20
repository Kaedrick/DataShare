const pptxgen = require('pptxgenjs');
const { warnIfSlideHasOverlaps, warnIfSlideElementsOutOfBounds } = require('/home/oai/skills/slides/pptxgenjs_helpers');

const pptx = new pptxgen();
pptx.layout = 'LAYOUT_WIDE';
pptx.author = 'DataShare';
pptx.subject = 'Présentation prototype DataShare';
pptx.title = 'DataShare - Prototype';
pptx.lang = 'fr-FR';
pptx.theme = {
  headFontFace: 'Aptos Display',
  bodyFontFace: 'Aptos',
  lang: 'fr-FR'
};
pptx.defineLayout({ name: 'LAYOUT_WIDE', width: 13.333, height: 7.5 });

function addFooter(slide, n) {
  slide.addText(`DataShare - MVP | ${n}`, { x: 10.8, y: 7.05, w: 2, h: 0.25, fontSize: 8, color: '777777' });
}
function title(slide, text) {
  slide.addText(text, { x: 0.75, y: 0.55, w: 11.9, h: 0.55, fontSize: 30, bold: true, color: '2E2730' });
}
function bullet(slide, lines, y, h = 4.8) {
  slide.addText(lines.map(t => ({ text: t, options: { bullet: { indent: 14 }, breakLine: true } })), { x: 1.05, y, w: 11.1, h, fontSize: 18, color: '2E2730', fit: 'shrink' });
}

let s = pptx.addSlide();
s.background = { color: 'FBE0D2' };
s.addText('DataShare', { x: 0.75, y: 2.25, w: 11.8, h: 0.8, fontSize: 48, bold: true, color: '2E2730', align: 'center' });
s.addText('Prototype de transfert sécurisé de fichiers', { x: 1.5, y: 3.25, w: 10.3, h: 0.4, fontSize: 22, color: '5A404D', align: 'center' });
s.addText('MVP fonctionnel : authentification, upload, liens temporaires, historique', { x: 1.25, y: 4.05, w: 10.8, h: 0.35, fontSize: 15, color: '5A404D', align: 'center' });
addFooter(s, 1); warnIfSlideHasOverlaps(s, pptx); warnIfSlideElementsOutOfBounds(s, pptx);

s = pptx.addSlide(); title(s, 'Objectif du prototype');
bullet(s, [
  'Permettre à un utilisateur connecté de déposer un fichier.',
  'Générer un lien temporaire de téléchargement.',
  'Afficher un historique personnel des fichiers déposés.',
  'Supprimer un fichier avant son expiration.'
], 1.6);
addFooter(s, 2); warnIfSlideHasOverlaps(s, pptx); warnIfSlideElementsOutOfBounds(s, pptx);

s = pptx.addSlide(); title(s, 'Architecture retenue');
s.addText('React / Vite', { x: 1, y: 1.7, w: 3.2, h: 0.45, fontSize: 24, bold: true });
s.addText('Interface utilisateur', { x: 1, y: 2.25, w: 3.2, h: 0.35, fontSize: 16 });
s.addText('ASP.NET Core API', { x: 5, y: 1.7, w: 3.4, h: 0.45, fontSize: 24, bold: true });
s.addText('Endpoints REST + JWT', { x: 5, y: 2.25, w: 3.4, h: 0.35, fontSize: 16 });
s.addText('PostgreSQL + fichiers locaux', { x: 8.9, y: 1.7, w: 3.8, h: 0.45, fontSize: 24, bold: true });
s.addText('Métadonnées en base, contenus dans /uploads', { x: 8.9, y: 2.25, w: 3.7, h: 0.6, fontSize: 16 });
bullet(s, ['Choix simple et cohérent avec le MVP.', 'Évolutif vers stockage cloud et tâche de purge planifiée.'], 4.1, 1.2);
addFooter(s, 3); warnIfSlideHasOverlaps(s, pptx); warnIfSlideElementsOutOfBounds(s, pptx);

s = pptx.addSlide(); title(s, 'Fonctionnalités implémentées');
bullet(s, [
  'US03/US04 : inscription et connexion avec JWT.',
  'US01 : upload connecté avec limite de taille et expiration.',
  'US02 : téléchargement via token unique.',
  'US05/US06 : historique personnel et suppression.',
  'Début optionnel : mot de passe de fichier et expiration configurable.'
], 1.45);
addFooter(s, 4); warnIfSlideHasOverlaps(s, pptx); warnIfSlideElementsOutOfBounds(s, pptx);

s = pptx.addSlide(); title(s, 'Sécurité et qualité');
bullet(s, [
  'Mots de passe hashés avec BCrypt.',
  'Routes utilisateur protégées par JWT.',
  'Contrôle de propriété avant suppression.',
  'Validation taille, durée et extensions de fichier.',
  'Documentation TESTING, SECURITY, PERF et MAINTENANCE.'
], 1.45);
addFooter(s, 5); warnIfSlideHasOverlaps(s, pptx); warnIfSlideElementsOutOfBounds(s, pptx);

s = pptx.addSlide(); title(s, 'Démonstration prévue');
bullet(s, [
  'Créer un compte puis se connecter.',
  'Téléverser un fichier avec expiration.',
  'Copier le lien généré.',
  'Consulter Mes fichiers.',
  'Tester le téléchargement puis la suppression.'
], 1.45);
addFooter(s, 6); warnIfSlideHasOverlaps(s, pptx); warnIfSlideElementsOutOfBounds(s, pptx);

pptx.writeFile({ fileName: '/mnt/data/datashare-student/docs/DataShare_presentation.pptx' });
