<template>
    <div class="section_exception text-center">
        <div class="bg-card box-shadow rounded-1 py-5 px-3">
            <div v-if="status == '401'" class="flex-center flex-column">
                <div class="flex-center">
                    <img width="150px" src="@/assets/images/icons/except_access.png" alt="">
                </div>
                <div class="flex-center flex-column mt-5">
                    <span class="text-color fs-secondary-medium fw-bold">401 - Access Error</span>
                    <span class="description-color fs-small fw-bold mt-3">Leave the desktop. Mobile gaming rocks!</span>
                </div>
            </div>
            <div v-else-if="status == '404'" class="flex-center flex-column">
                <div class="flex-center">
                    <img width="150px" src="@/assets/images/icons/except_found.png" alt="">
                </div>
                <div class="flex-center flex-column mt-5">
                    <span class="text-color fs-secondary-medium fw-bold">404 - Not Found</span>
                    <span class="description-color fs-small fw-bold mt-3">Can not found page or object in server</span>
                </div>
                <div @click="$router.push({ name: 'index' })"
                    class="bg-theme p-2 rounded-1 box-shadow w-full flex-center mt-5">
                    <span class="text-color fs-medium fw-bold">Back To Home</span>
                </div>
            </div>
            <div v-else class="flex-center flex-column">
                <div class="flex-center">
                    <img width="150px" src="@/assets/images/icons/except_server.png" alt="">
                </div>
                <div class="flex-center flex-column mt-5">
                    <span class="text-color fs-secondary-medium fw-bold">500 - Internal Server</span>
                    <span class="description-color fs-small fw-bold mt-3">
                        An unknown problem has occurred on the server side
                    </span>
                </div>
                <div @click="$router.push({ name: 'index' })"
                    class="bg-theme p-2 rounded-1 box-shadow w-full flex-center mt-5">
                    <span class="text-color fs-medium fw-bold">Back To Home</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router';

const route = useRoute();

const status = route.params.status.toString();

</script>