<template>
  <div class="page-container">
    <div class="cards-container">
      <CardBase
        v-for="exhibition in exhibitions"
        :key="exhibition.id"
        :item="exhibition"
      >
        <template #extra>
          <div class="extra-row extra-block">
            <div class="extra-left"></div>
            <div class="extra-right">
              <span class="extra-label">Home Page</span>
              <label class="switch">
                <input type="checkbox" v-model="exhibition.highlighted" />
                <span class="slider round"></span>
              </label>
            </div>
          </div>

          <div class="fs-large">{{ exhibition.name }}</div>
          <div class="fs-medium">{{ exhibition.description }}</div>
          <div class="fs-small">{{ exhibition.type }}</div>
          <div class="fs-small">
            Stand associati: {{ exhibition.numberOfStands }}
          </div>

          <StateChip :state="exhibition.state" />
        </template>
      </CardBase>
    </div>

    <div v-if="exhibitions.length === 0" class="empty-placeholder">
      Nessuna area espositiva disponibile
    </div>
  </div>
</template>

<script lang="ts" setup>
import CardBase from './CardBase.vue';
import StateChip from './StateChip.vue';

interface Exhibition {
  id: number;
  name: string;
  description?: string;
  imageUrl?: string | null;
  numberOfStands: number;
  lastModify: string;
  modifyBy: string;
  type: string;
  state?: string;
  highlighted: boolean;
}

// Riceve dati da HomePage tramite prop
defineProps<{
  exhibitions: Exhibition[];
}>();
</script>

<style src="../styles/components/ExhibitionAreaCard.css"></style>