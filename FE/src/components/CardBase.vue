<template>
  <div class="card">
    <!-- Immagine o placeholder -->
    <div class="image-wrapper">
      <img v-if="item.imageUrl" :src="item.imageUrl" class="card-image" />
      <div v-else class="placeholder-x">X</div>
    </div>

    <!-- Slot extra per dettagli specifici dell'entity -->
    <slot name="extra"></slot>

    <!-- Audit info -->
    <div class="audit-info">
      {{ item.lastModify }}<br />{{ item.modifyBy }}
    </div>
  </div>
</template>

<script lang="ts" setup>
// Usa defineProps in questo modo
interface Item {
  name: string;
  imageUrl?: string | null;
  lastModify: string;
  modifyBy: string;
}

const props = defineProps<{
  item: Item;
}>();
</script>