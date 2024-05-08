<template>
    <div class="d-flex align-items-center justify-content-between">
        <div class="d-flex flex-column">
            <span class="text-color fs-medium fw-bold">{{ props.title }}</span>
            <span class="description-color fs-small fw-normal mt-1">{{ props.description }}</span>
        </div>
        <div class="search_box rounded-1 bg-card box-shadow p-1 flex-center">
            <SearchIcon class="mr-2 text-color" />
            <input v-model="query" @input="changeSearchData" class="text-color fs-small fw-normal" type="text"
                placeholder="search ...">
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import SearchIcon from '@/components/icons/SearchIcon.vue'

interface Props {
    title: string,
    description?: string,
}

const props = defineProps<Props>();

const query = ref<string>("");

const emit = defineEmits<{
    (e: 'change', query: string): void
}>()


const changeSearchData = () => {
    emit('change', query.value)
};
</script>

<style scoped>
.search_box input {
    width: 120px;
}


</style>