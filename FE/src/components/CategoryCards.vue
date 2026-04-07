<template>
  <div class="page-container">
    <div class="cards-container">
      <CardBase
        v-for="category in categories"
        :key="category.id"
        :item="category"
      >
        <template #extra>
          <div class="extra-row extra-block">
            <div class="extra-left"></div>
            <div class="extra-right">
              <span class="extra-label">Home Page</span>
              <label class="switch">
                <input type="checkbox" v-model="category.highlighted" />
                <span class="slider round"></span>
              </label>
            </div>
          </div>

          <div class="fs-large">{{ category.name }}</div>
          <div class="fs-medium">{{ category.description }}</div>
        </template>
      </CardBase>
    </div>

    <div v-if="categories.length === 0" class="empty-placeholder">
      Nessuna categoria disponibile
    </div>
  </div>
</template>

<script lang="ts" setup>
import CardBase from './CardBase.vue';

interface Category {
  id: number;
  name: string;
  imageUrl?: string | null;
  description?: string | null;
  lastModify: string;
  modifyBy: string;
  highlighted: boolean;
}

// Riceve i dati da HomePage tramite prop
defineProps<{
  categories: Category[];
}>();
</script>

<style src="../styles/components/CategoryCard.css"></style>