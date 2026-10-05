/*
 * build-rae-dict.js — Expande los lemas RAE (rla-es) a formas flexionadas.
 *
 * Lee los ficheros de lemas de dev/rae/*.txt (diccionario de la RAE con
 * acentos y ñ) y emite una lista plana de palabras flexionadas:
 *   - verbos regulares (flags RED, REDA, REID...) -> conjugación completa
 *   - verbos irregulares (IRD/IR/XD/REID) -> infinitivo + formas de la tabla
 *   - nombres/adjetivos                   -> plural y femenino
 *
 * Uso:
 *   node dev/build-rae-dict.js <salida.txt>
 */
"use strict";

const fs = require("fs");
const path = require("path");

const DIR = path.join(__dirname, "rae");
const WORD_REGEX = /^\p{L}+$/u;

const UNACCENT = { á: "a", é: "e", í: "i", ó: "o", ú: "u" };
function unaccent(ch) { return UNACCENT[ch] || ch; }

/* Conjugación regular */
const CONJ = {
    ar: {
        present: ["o", "as", "", "amos", "áis", "an"],
        preterite: ["é", "aste", "ó", "amos", "asteis", "aron"],
        imperfect: ["aba", "abas", "aba", "ábamos", "abais", "aban"],
        subj: ["e", "es", "e", "emos", "éis", "en"],
        gerund: "ando",
        part: "ado"
    },
    er: {
        present: ["o", "es", "e", "emos", "éis", "en"],
        preterite: ["í", "iste", "ió", "imos", "isteis", "ieron"],
        imperfect: ["ía", "ías", "ía", "íamos", "íais", "ían"],
        subj: ["a", "as", "a", "amos", "áis", "an"],
        gerund: "iendo",
        part: "ido"
    },
    ir: {
        present: ["o", "es", "e", "imos", "ís", "en"],
        preterite: ["í", "iste", "ió", "imos", "isteis", "ieron"],
        imperfect: ["ía", "ías", "ía", "íamos", "íais", "ían"],
        subj: ["a", "as", "a", "amos", "áis", "an"],
        gerund: "iendo",
        part: "ido"
    }
};

const FUTURE = ["é", "ás", "á", "emos", "éis", "án"];
const CONDITIONAL = ["ía", "ías", "ía", "íamos", "íais", "ían"];

/* Formas irregulares de verbos muy frecuentes (RAE). */
const IRREG = {
    ser: ["soy", "eres", "es", "somos", "sois", "son", "era", "eras", "era", "éramos", "erais", "eran",
        "sea", "seas", "sea", "seamos", "seáis", "sean", "fui", "fuiste", "fue", "fuimos", "fuisteis", "fueron",
        "seré", "serás", "será", "seremos", "seríais", "serían", "sido", "siendo", "fueran", "fuéramos"],
    estar: ["estoy", "estás", "está", "estamos", "estáis", "están", "estaba", "estabas", "estaba",
        "estábamos", "estaban", "esté", "estés", "esté", "estemos", "estéis", "estén",
        "estuve", "estuviste", "estuvo", "estuvimos", "estuvieron", "estaba", "estado", "estando"],
    ir: ["voy", "vas", "va", "vamos", "vais", "van", "iba", "ibas", "iba", "íbamos", "ibais", "iban",
        "vaya", "vayas", "vaya", "vayamos", "vayáis", "vayan", "fui", "fuiste", "fue", "fuimos", "fuisteis", "fueron",
        "id", "ido", "yendo", "iré", "irás", "irá", "iremos", "iréis", "irán"],
    haber: ["he", "has", "ha", "hemos", "habéis", "han", "había", "habías", "había", "habíamos", "habían",
        "haya", "hayas", "haya", "hayamos", "hayáis", "hayan", "hubo", "hubiera", "hubieras", "hubieran",
        "habré", "habrás", "habrá", "habremos", "habido", "habiendo"],
    hacer: ["hago", "haces", "hace", "hacemos", "hacéis", "hacen", "hacía", "hacías", "hacía", "hacíamos", "hacían",
        "haga", "hagas", "haga", "hagamos", "hagáis", "hagan", "hice", "hiciste", "hizo", "hicimos", "hicieron",
        "haré", "harás", "hará", "haremos", "hecho", "haciendo"],
    tener: ["tengo", "tienes", "tiene", "tenemos", "tenéis", "tienen", "tenía", "tenías", "tenía", "teníamos", "tenían",
        "tenga", "tengas", "tenga", "tengamos", "tengan", "tuve", "tuviste", "tuvo", "tuvimos", "tuvieron",
        "tendré", "tendrás", "tendrá", "tendremos", "tenido", "teniendo"],
    poder: ["puedo", "puedes", "puede", "podemos", "podéis", "pueden", "podía", "podías", "podía", "podíamos", "podían",
        "pueda", "puedas", "pueda", "podamos", "puedan", "pude", "pudiste", "pudo", "pudimos", "pudieron",
        "podré", "podrás", "podrá", "podremos", "podido", "pudiendo"],
    querer: ["quiero", "quieres", "quiere", "queremos", "queréis", "quieren", "quería", "querías", "quería", "queríamos", "querían",
        "quiera", "quieras", "quiera", "queramos", "quieran", "quise", "quisiste", "quiso", "quisimos", "quisieron",
        "querré", "querrás", "querrá", "querremos", "querido", "queriendo"],
    saber: ["sé", "sabes", "sabe", "sabemos", "sabéis", "saben", "sabía", "sabías", "sabía", "sabíamos", "sabían",
        "sepa", "sepas", "sepa", "sepamos", "sepan", "supe", "supiste", "supo", "supimos", "supieron",
        "sabré", "sabrás", "sabrá", "sabremos", "sabido", "sabiendo"],
    decir: ["digo", "dices", "dice", "decimos", "decís", "dicen", "decía", "decías", "decía", "decíamos", "decían",
        "diga", "digas", "diga", "digamos", "digáis", "digan", "dije", "dijiste", "dijo", "dijimos", "dijeron",
        "diré", "dirás", "dirá", "diremos", "dicho", "diciendo"],
    venir: ["vengo", "vienes", "viene", "venimos", "venís", "vienen", "venía", "venías", "venía", "veníamos", "venían",
        "venga", "vengas", "venga", "vengamos", "vengan", "vine", "viniste", "vino", "vinimos", "vinieron",
        "vendré", "vendrás", "vendrá", "vendremos", "venido", "viniendo"],
    poner: ["pongo", "pones", "pone", "ponemos", "ponéis", "ponen", "ponía", "ponías", "ponía", "poníamos", "ponían",
        "ponga", "pongas", "ponga", "pongamos", "pongan", "puse", "pusiste", "puso", "pusimos", "pusieron",
        "pondré", "pondrás", "pondrá", "pondremos", "puesto", "poniendo"],
    ver: ["veo", "ves", "ve", "vemos", "veis", "ven", "veía", "veías", "veía", "veíamos", "veían",
        "vea", "veas", "vea", "veamos", "veáis", "vean", "vi", "viste", "vio", "vimos", "vieron",
        "veré", "verás", "verá", "veremos", "visto", "viendo"],
    dar: ["doy", "das", "da", "damos", "dais", "dan", "daba", "dabas", "daba", "dábamos", "daban",
        "dé", "des", "dé", "demos", "deis", "den", "di", "distes", "dio", "dimos", "dieron",
        "daré", "darás", "dará", "daremos", "dado", "dando"],
    salir: ["salgo", "sales", "sale", "salimos", "salís", "salen", "salía", "salías", "salía", "salíamos", "salían",
        "salga", "salgas", "salga", "salgamos", "salgan", "salí", "saliste", "salió", "salimos", "salieron",
        "saldré", "saldrás", "saldrá", "saldremos", "salido", "saliendo"],
    traer: ["traigo", "traes", "trae", "traemos", "traéis", "traen", "traía", "traías", "traía", "traíamos", "traían",
        "traiga", "traigas", "traiga", "traigamos", "traigan", "traje", "trajiste", "trajo", "trajimos", "trajeron",
        "traeré", "traerás", "traerá", "traeremos", "traído", "trayendo"],
    conocer: ["conozco", "conoces", "conoce", "conocemos", "conocéis", "conocen", "conocía", "conocía", "conocían",
        "conozca", "conozcas", "conozca", "conozcamos", "conozcan", "conocí", "conociste", "conoció", "conocido", "conociendo"],
    oir: ["oigo", "oyes", "oye", "oímos", "oís", "oyen", "oía", "oías", "oía", "oíamos", "oían",
        "oiga", "oigas", "oiga", "oigamos", "oigan", "oí", "oíste", "oyó", "oímos", "oyeron",
        "oído", "oyendo", "oiré", "oirás", "oirá", "oiremos"],
    leer: ["leo", "lees", "lee", "leemos", "leéis", "leen", "leía", "leías", "leía", "leíamos", "leían",
        "lea", "leas", "lea", "leamos", "leáis", "lean", "leí", "leíste", "leyó", "leímos", "leyeron",
        "leído", "leyendo", "leeré", "leerás", "leerá", "leeremos"],
    creer: ["creo", "crees", "cree", "creemos", "creéis", "creen", "creía", "creías", "creía", "creíamos", "creían",
        "crea", "creas", "crea", "creamos", "crean", "creí", "creíste", "creyó", "creímos", "creyeron",
        "creído", "creyendo", "creeré", "creerás", "creerá", "creeremos"],
    sentir: ["siento", "sientes", "siente", "sentimos", "sentís", "sienten", "sentía", "sentías", "sentía", "sentíamos", "sentían",
        "sienta", "sientas", "sienta", "sintamos", "sientan", "sentí", "sentiste", "sintió", "sentimos", "sintieron",
        "sintiendo", "sentido"],
    dormir: ["duermo", "duermes", "duerme", "dormimos", "dormís", "duermen", "dormía", "dormías", "dormía", "dormíamos", "dormían",
        "duerma", "duermas", "duerma", "durmamos", "duerman", "dormí", "dormiste", "durmió", "dormimos", "durmieron",
        "durmiendo", "dormido"],
    pedir: ["pido", "pides", "pide", "pedimos", "pedís", "piden", "pedía", "pedías", "pedía", "pedíamos", "pedían",
        "pida", "pidas", "pida", "pidamos", "pidan", "pedí", "pediste", "pidió", "pedimos", "pidieron",
        "pidiendo", "pedido"],
    seguir: ["sigo", "sigues", "sigue", "seguimos", "seguís", "siguen", "seguía", "seguías", "seguía", "seguíamos", "seguían",
        "siga", "sigas", "siga", "sigamos", "sigan", "seguí", "seguiste", "siguió", "seguimos", "siguieron",
        "siguiendo", "seguido"],
    servir: ["sirvo", "sirves", "sirve", "servimos", "servís", "sirven", "servía", "servías", "servía", "servíamos", "servían",
        "sirva", "sirvas", "sirva", "sirvamos", "sirvan", "serví", "serviste", "sirvió", "servimos", "sirvieron",
        "sirviendo", "servido"],
    preferir: ["prefiero", "prefieres", "prefiere", "preferimos", "preferís", "prefieren", "prefería", "prefería", "preferían",
        "prefiera", "prefieras", "prefiera", "prefiera", "prefieran", "preferí", "preferiste", "prefirió", "preferimos", "prefirieron",
        "prefiriendo", "preferido"],
    jugar: ["juego", "juegas", "juega", "jugamos", "jugáis", "juegan", "jugaba", "jugabas", "jugaba", "jugábamos", "jugaban",
        "juegue", "juegues", "juegue", "juguemos", "jueguen", "jugué", "jugaste", "jugó", "jugamos", "jugaron",
        "jugando", "jugado"],
    valer: ["valgo", "vales", "vale", "valemos", "valéis", "valen", "valía", "valías", "valía", "valíamos", "valían",
        "valga", "valgas", "valga", "valgamos", "valgan", "valí", "valiste", "valió", "valimos", "valieron",
        "valdré", "valdrás", "valdrá", "valdremos", "valido", "valiendo"],
    huir: ["huyo", "huyes", "huye", "huimos", "huís", "huyen", "huía", "huías", "huía", "huíamos", "huían",
        "huya", "huyas", "huya", "huyamos", "huyan", "huí", "huiste", "huyó", "huimos", "huyeron",
        "huyendo", "huido"],
    construir: ["construyo", "construyes", "construye", "construimos", "construís", "construyen", "construía", "construía", "construían",
        "construya", "construyas", "construya", "construyamos", "construyan", "construí", "construiste", "construyó", "construimos", "construyeron",
        "construyendo", "construido"],
    incluir: ["incluyo", "incluyes", "incluye", "incluimos", "incluís", "incluyen", "incluía", "incluía", "incluían",
        "incluya", "incluyas", "incluya", "incluyamos", "incluyan", "incluí", "incluiste", "incluyó", "incluimos", "incluyeron",
        "incluyendo", "incluido"],
    caber: ["quepo", "cabes", "cabe", "cabemos", "cabéis", "caben", "cabía", "cabías", "cabía", "cabíamos", "cabían",
        "quepa", "quepas", "quepa", "quepamos", "quepan", "cupe", "cupiste", "cupo", "cupimos", "cupieron",
        "cabré", "cabrás", "cabrá", "cabremos", "cabido", "cabiendo"],
    andar: ["ando", "andas", "anda", "andamos", "andáis", "andan", "andaba", "andabas", "andaba", "andábamos", "andaban",
        "ande", "andes", "ande", "andemos", "andéis", "anden", "anduve", "anduviste", "anduvo", "anduvimos", "anduvieron",
        "andando", "andado"],
    caer: ["caigo", "caes", "cae", "caemos", "caéis", "caen", "caía", "caías", "caía", "caíamos", "caían",
        "caiga", "caigas", "caiga", "caigamos", "caigan", "caí", "caíste", "cayó", "caímos", "cayeron",
        "cayendo", "caído"]
};

function parseBase(line) {
    let s = line.replace(/\r$/, "").trim();
    if (!s || s.startsWith("#")) return null;
    const slash = s.indexOf("/");
    if (slash !== -1) s = s.slice(0, slash);
    s = s.trim().toLowerCase();
    if (!WORD_REGEX.test(s)) return null;
    return s;
}

function hasFlag(line, re) {
    const m = line.match(/\/([A-Z]+)[^\s]*\s*$/);
    return m ? re.test(m[1]) : false;
}

function plural(word, out) {
    if (word.length < 4) return;
    if (word.endsWith("s") || word.endsWith("x")) return;
    if (word.endsWith("z")) {
        out.add(word.slice(0, -1) + "ces");
        return;
    }
    out.add(word + "es");
    if (word.endsWith("ón")) out.add(word.slice(0, -2) + "ones");
    if (word.endsWith("án")) out.add(word.slice(0, -2) + "anes");
}

function feminine(word, out) {
    if (word.length < 4) return;
    if (word.endsWith("o")) out.add(word.slice(0, -1) + "a");
}

function addVerb(words, base, flagStr) {
    let stem, type;
    if (base.endsWith("ar")) { stem = base.slice(0, -2); type = "ar"; }
    else if (base.endsWith("er")) { stem = base.slice(0, -2); type = "er"; }
    else if (base.endsWith("ir")) { stem = base.slice(0, -2); type = "ir"; }
    else return false;

    const c = CONJ[type];
    const add = w => { if (w) words.add(w); };

    add(base);
    for (const e of c.present) add(stem + e);
    for (const e of c.preterite) add(stem + e);
    for (const e of c.imperfect) add(stem + e);
    for (const e of c.subj) add(stem + e);
    for (const e of FUTURE) add(base + e);
    for (const e of CONDITIONAL) add(base + e);
    add(stem + c.gerund);
    add(stem + c.part);
    return true;
}

function build() {
    const verbs = new Set();   // derivadas de verbos (no reciben plural/femenino genérico)
    const nouns = new Set();   // nombres, adjetivos y demás (reciben flexión genérica)
    const files = fs.readdirSync(DIR).filter(f => f.endsWith(".txt"));
    const REG_VERB = /^(RED|REID|REDA|REDT|REDTA|REDA|REP|REIDA|RE|ARED|RD)/;
    const IRREG_VERB = /^(IRDT|IRD|IR|XDT|XDA|XD|X|D)/;

    for (const f of files) {
        const text = fs.readFileSync(path.join(DIR, f), "utf8");
        for (const line of text.split(/\r?\n/)) {
            const base = parseBase(line);
            if (!base) continue;

            const isVerb = hasFlag(line, REG_VERB);
            if (isVerb) {
                if (addVerb(verbs, base, "")) { /* conjugado */ }
                else verbs.add(base);
            } else if (hasFlag(line, IRREG_VERB)) {
                verbs.add(base);
            } else {
                nouns.add(base);
            }

            const ir = Object.prototype.hasOwnProperty.call(IRREG, base) ? IRREG[base] : undefined;
            if (ir) ir.forEach(w => verbs.add(w));
        }
    }

    // Flexión genérica SOLO sobre nombres/adjetivos, y solo palabras >= 4 letras
    const nounBase = Array.from(nouns);
    for (const w0 of nounBase) {
        plural(w0, nouns);
        feminine(w0, nouns);
    }

    const words = new Set(verbs);
    for (const w of nouns) words.add(w);
    return words;
}

if (require.main === module) {
    const out = process.argv[2];
    const words = build();
    const sorted = Array.from(words).sort();
    if (out) fs.writeFileSync(out, sorted.join("\n") + "\n", "utf8");
    console.log("OK", sorted.length, "palabras flexionadas ->", out || "(stdout)");
}

module.exports = { build };