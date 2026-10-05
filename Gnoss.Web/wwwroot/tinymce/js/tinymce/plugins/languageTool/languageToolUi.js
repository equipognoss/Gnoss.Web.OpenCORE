/* languageToolUi.js
 * Modal "palabra por palabra" estilo Word para revisar errores del corrector.
 * API:
 *   UI.openModal(state) -> { render(), close() }
 *      state: { editor, text, matches[], index(), setIndex(i),
 *               onApply(match, replacement), onIgnore(match), onIgnoreAll(match), onEmpty() }
 *   text puede ser un string o una función que devuelve el texto ACTUAL (lo
 *   usa el plugin para que el contexto se recalcule tras corregir errores).
 *   matches puede ser un array o una función que devuelve el array ACTUAL de
 *   errores — el plugin sustituye el array entero tras cada re-check, y el
 *   modal debe renderizar siempre contra el array vigente (no el inicial), o
 *   el contador "Error X de N" se quedaría desfasado al corregir.
 *   UI.setButtonState(buttonApi, checking, label)
 */
(function (global) {
    "use strict";

    const ns = (global.LanguageTool = global.LanguageTool || {});

    function contextParts(text, offset, length, radius) {
        const beforeRaw = text.slice(Math.max(0, offset - radius), offset);
        const afterRaw = text.slice(offset + length, offset + length + radius);
        return {
            before: (offset - radius > 0 ? "…" : "") + beforeRaw,
            word: text.slice(offset, offset + length),
            after: afterRaw + (offset + length + radius < text.length ? "…" : "")
        };
    }

    function setButtonState(buttonApi, checking, label) {
        if (!buttonApi) return;
        try {
            buttonApi.setDisabled(!!checking);
            if (label && buttonApi.setText) buttonApi.setText(label);
        } catch (e) { /* noop */ }
    }

    function openModal(state) {
        const d = global.document;
        const overlay = d.createElement("div");
        overlay.className = "lt-overlay";

        const modal = d.createElement("div");
        modal.className = "lt-modal";
        overlay.appendChild(modal);

        const header = d.createElement("div");
        header.className = "lt-modal-header";
        const title = d.createElement("span");
        title.className = "lt-modal-title";
        title.textContent = "Corrección ortográfica";
        const closeBtn = d.createElement("button");
        closeBtn.className = "lt-modal-close";
        closeBtn.textContent = "×";
        closeBtn.setAttribute("aria-label", "Cerrar");
        header.appendChild(title);
        header.appendChild(closeBtn);
        modal.appendChild(header);

        const body = d.createElement("div");
        body.className = "lt-modal-body";

        const progress = d.createElement("div");
        progress.className = "lt-progress";

        const context = d.createElement("div");
        context.className = "lt-context";
        const ctxBefore = d.createElement("span");
        ctxBefore.className = "lt-ctx-before";
        const ctxWord = d.createElement("span");
        ctxWord.className = "lt-ctx-word";
        const ctxAfter = d.createElement("span");
        ctxAfter.className = "lt-ctx-after";
        context.appendChild(ctxBefore);
        context.appendChild(ctxWord);
        context.appendChild(ctxAfter);

        const msg = d.createElement("div");
        msg.className = "lt-msg";
        const cat = d.createElement("div");
        cat.className = "lt-cat";

        const suggWrap = d.createElement("div");
        suggWrap.className = "lt-suggestions";

        const actions = d.createElement("div");
        actions.className = "lt-actions";
        const btnAccept = d.createElement("button");
        btnAccept.className = "lt-btn lt-btn-primary";
        btnAccept.textContent = "Aceptar";
        const btnIgnore = d.createElement("button");
        btnIgnore.className = "lt-btn";
        btnIgnore.textContent = "Ignorar";
        const btnIgnoreAll = d.createElement("button");
        btnIgnoreAll.className = "lt-btn";
        btnIgnoreAll.textContent = "Ignorar todas";
        const btnPrev = d.createElement("button");
        btnPrev.className = "lt-btn lt-btn-nav";
        btnPrev.textContent = "← Anterior";
        const btnNext = d.createElement("button");
        btnNext.className = "lt-btn lt-btn-nav";
        btnNext.textContent = "Siguiente →";
        actions.appendChild(btnAccept);
        actions.appendChild(btnIgnore);
        actions.appendChild(btnIgnoreAll);
        actions.appendChild(btnPrev);
        actions.appendChild(btnNext);

        body.appendChild(progress);
        body.appendChild(context);
        body.appendChild(msg);
        body.appendChild(cat);
        body.appendChild(suggWrap);
        body.appendChild(actions);
        modal.appendChild(body);

        d.body.appendChild(overlay);

        let closed = false;

        function getMatches() {
            return typeof state.matches === "function" ? state.matches() : state.matches;
        }

        function current() {
            const ms = getMatches();
            return ms && ms[state.index()] || null;
        }

        function render() {
            if (closed) return;
            const matches = getMatches();
            const total = matches.length;
            const text = typeof state.text === "function" ? state.text() : state.text;
            const m = current();
            if (!m) {
                progress.textContent = total === 0 ? "" : "";
                context.style.display = "none";
                cat.style.display = "none";
                suggWrap.textContent = "";
                msg.className = "lt-msg lt-empty";
                msg.style.display = "";
                msg.textContent = total === 0
                    ? "No hay errores de ortografía. El texto está correcto."
                    : "No hay más errores.";
                btnAccept.disabled = true;
                btnIgnore.disabled = true;
                btnIgnoreAll.disabled = true;
                btnPrev.disabled = true;
                btnNext.disabled = true;
                return;
            }
            const i = state.index();
            progress.textContent = "Error " + (i + 1) + " de " + total;

            context.style.display = "";
            const parts = contextParts(text, m.offset, m.length, 40);
            ctxBefore.textContent = parts.before;
            ctxWord.textContent = parts.word;
            ctxAfter.textContent = parts.after;

            msg.style.display = "";
            msg.className = "lt-msg";
            msg.textContent = m.message || "";

            cat.style.display = "";
            cat.textContent = m.ruleDescription
                ? m.ruleDescription + (m.category ? " · " + m.category : "")
                : (m.category || "");

            suggWrap.textContent = "";
            const repls = m.replacements || [];
            repls.forEach(function (r) {
                const b = d.createElement("button");
                b.className = "lt-suggestion";
                b.textContent = r;
                b.addEventListener("click", function () {
                    if (state.onApply) state.onApply(m, r);
                });
                suggWrap.appendChild(b);
            });
            if (!repls.length) {
                const hint = d.createElement("span");
                hint.className = "lt-no-sugg";
                hint.textContent = "Sin sugerencias";
                suggWrap.appendChild(hint);
            }

            btnAccept.disabled = repls.length === 0;
            btnIgnore.disabled = false;
            btnIgnoreAll.disabled = false;
            btnPrev.disabled = i <= 0;
            btnNext.disabled = i >= total - 1;
        }

        function close() {
            if (closed) return;
            closed = true;
            d.removeEventListener("keydown", keyHandler, true);
            if (overlay.parentNode) overlay.parentNode.removeChild(overlay);
            if (state.onClose) state.onClose();
        }

        function keyHandler(e) {
            if (e.key === "Escape") {
                e.stopPropagation();
                close();
            }
        }
        d.addEventListener("keydown", keyHandler, true);

        closeBtn.addEventListener("click", close);

        btnAccept.addEventListener("click", function () {
            const m = current();
            if (!m || !m.replacements || !m.replacements.length) return;
            if (state.onApply) state.onApply(m, m.replacements[0]);
        });
        btnIgnore.addEventListener("click", function () {
            const m = current();
            if (!m) return;
            if (state.onIgnore) state.onIgnore(m);
        });
        btnIgnoreAll.addEventListener("click", function () {
            const m = current();
            if (!m) return;
            if (state.onIgnoreAll) state.onIgnoreAll(m);
        });
        btnPrev.addEventListener("click", function () {
            if (state.setIndex) state.setIndex(Math.max(0, state.index() - 1));
            render();
        });
        btnNext.addEventListener("click", function () {
            if (state.setIndex) state.setIndex(Math.min(getMatches().length - 1, state.index() + 1));
            render();
        });

        render();

        return {
            render: render,
            close: close
        };
    }

    ns.UI = {
        openModal,
        setButtonState,
        contextParts
    };
})(typeof window !== "undefined" ? window : globalThis);