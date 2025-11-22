<template>
  <main class="app-shell">
    <section class="app-header">
      <h1>Bid Calculation Tool</h1>      
    </section>

    <section class="app-content">
      <BidCalculatorView />
    </section>
  </main>
</template>

<script setup>
import BidCalculatorView from "./components/BidCalculatorView.vue";
</script>

<style scoped>
.app-shell {
  max-width: 800px;
  margin: 0 auto;
  padding: 1.5rem;
}

.app-header h1 {
  margin-bottom: 0.25rem;
}

.app-header p {
  margin: 0;
  font-size: 0.95rem;
  color: #555;
}

.app-content {
  margin-top: 1.5rem;
}
</style>
