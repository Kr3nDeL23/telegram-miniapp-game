<template>
    <div :style="`width:${props.width};height:${props.height};`" class="box-shadow skeleton rounded-1"></div>
</template>
<script setup lang="ts">
interface Props {
    width: string
    height: string
}

const props = defineProps<Props>()

</script>

<style scoped>
@keyframes SkeletonLoading {
    0% {
        background-color: hsla(var(--theme-color-light), 0.3);
    }

    100% {
        background-color: hsla(var(--theme-color), 0.2);
    }
}

.skeleton {
    animation: SkeletonLoading var(--transition-slow) linear infinite alternate;
}
</style>