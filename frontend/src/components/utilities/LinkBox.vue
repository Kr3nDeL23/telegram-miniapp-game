<template>
    <div class="bg-card box-shadow rounded-1 p-2">
        <div class="d-flex align-items-center justify-content-between">
            <div class="d-flex align-items-center input_container">
                <div class="flex-center mr-2">
                    <img class="link_image" src="@/assets/images/icons/link.png" alt="">
                </div>
                <div class="flex-center w-full input_link_container">
                    <input class="w-full text-left input_link text-color fs-small fw-normal" type="text" v-model="link"
                        @input="preventWrite">
                </div>
            </div>
            <div class="flex-center" v-on:click="copyLink">
                <img class="copy_link_image" src="@/assets/images/icons/copy.png" alt="">
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
interface Props {
    link: string
}

const props = defineProps<Props>()

const link = ref<string>(props.link);

const copyLink = () => {
    const input_link = document.querySelector('.input_link')

    if (!input_link) return;

    input_link.select()
};

const preventWrite = (e: any) => link.value = props.link;

</script>

<style scoped>
.input_link {
    letter-spacing: 1px;
}

.input_container {
    width: 90%;
}

.link_image,
.copy_link_image {
    min-width: 28px;
    max-width: 28px;
}

.input_link_container {
    overflow-x: auto;
}
</style>