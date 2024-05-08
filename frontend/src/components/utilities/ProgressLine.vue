<template>
    <div class="progress_line w-full bg-card box-shadow p-1 rounded-1">
        <div :style="`width:${getPercentFill()}%;`" class="progress_line_fill rounded-1"></div>
    </div>
</template>

<script setup lang="ts">
interface Props {
    size: number
    value: number
}

const props = defineProps<Props>()

const getPercentFill = () => {
    return (props.value / props.size) * 100;
};
</script>

<style scoped>
.progress_line {
    overflow: hidden;
}

.progress_line_fill {
    height: 100%;
    background-color: hsl(var(--theme-color));
}
</style>