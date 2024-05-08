import { createApp } from 'vue'

import App from './App.vue'
import router from './router'
import { BASE_URL } from './configurations/HttpConfiguration'

const app = createApp(App)

app.use(router)

app.config.globalProperties.$filters = {
    truncate(value: string = "", maxLength: number = 0) {
        if (value.length > maxLength) {
            return value.substring(0, maxLength) + '...';
        }
        return value;
    },
    numberFormat(value: number = 0) {
        if (Number.isInteger(value)) {
            return value.toLocaleString();
        } else {
            return value.toLocaleString(undefined, {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
        }
    },
    serverLinkFormat(link: string = "") {
        return BASE_URL + link;
    },
}

app.mount('#app')
