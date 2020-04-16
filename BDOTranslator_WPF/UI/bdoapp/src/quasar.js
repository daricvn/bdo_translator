import Vue from "vue";

import "./styles/quasar.sass";
import "@quasar/extras/roboto-font/roboto-font.css";
import "@quasar/extras/material-icons/material-icons.css";
import "@quasar/extras/fontawesome-v5/fontawesome-v5.css";
import {
  Quasar,
  QLayout,
  QBtn,
  QPageContainer,
  QInput,
  QVirtualScroll,
  QItem,
  QItemLabel,
  QItemSection,
  QMenu,
  QBar,
  QIcon,
  Loading,
  QSeparator,
  QSpace,
  QTooltip,
  QCheckbox,
  QDialog,
  QCard,
  QCardSection,
  QCardActions,
  QSelect,
  ClosePopup,
  Dialog,
  Notify
} from "quasar";

Vue.use(Quasar, {
  config: {},
  components: {
    QLayout,
    QPageContainer,
    QInput,
    QVirtualScroll,
    QItem,
    QItemLabel,
    QItemSection,
    QIcon,
    QMenu,
    QBar,
    QSeparator,
    QTooltip,
    QCheckbox,
    QSelect,
    QCard,
    QCardSection,
    QCardActions,
    QDialog,
    QSpace,
    QBtn
  },
  directives: {
    ClosePopup
  },
  plugins: {
    Dialog,
    Loading,
    Notify
  }
});
