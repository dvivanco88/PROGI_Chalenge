<template>
  <div class="card">
    <div class="card-header">
      <h2>Bid details</h2>
      <p>Just enter a price and pick a type, the rest updates on its own.</p>
    </div>

    <div class="card-body">
      <form class="form-grid" @submit.prevent>
        
        <div class="form-field">
          <label for="price">Base price</label>
          <input
            id="price"
            type="number"
            min="0"
            step="0.01"
            v-model.number="price"
            placeholder="Example: 1000"
          />
        </div>

        <div class="form-field">
          <label for="type">Type</label>
          <select id="type" v-model="vehicleType">
            <option value="Common">Common</option>
            <option value="Luxury">Luxury</option>
          </select>
        </div>

      </form>

      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      <p v-if="isLoading" class="hint">Calculating...</p>

      <section v-if="hasResult" class="fees">
        <h3>Fees breakdown</h3>
        <ul>
          <li><span>Base</span><span>{{ formatCurrency(fees.price) }}</span></li>
          <li><span>Buyer fee</span><span>{{ formatCurrency(fees.basicBuyerFee) }}</span></li>
          <li><span>Seller fee</span><span>{{ formatCurrency(fees.sellerSpecialFee) }}</span></li>
          <li><span>Association</span><span>{{ formatCurrency(fees.associationFee) }}</span></li>
          <li><span>Storage</span><span>{{ formatCurrency(fees.storageFee) }}</span></li>
        </ul>

        <div class="total">
          <span>Total</span>
          <strong>{{ formatCurrency(fees.total) }}</strong>
        </div>
      </section>
    </div>
  </div>
</template>


<script setup>
import { ref, reactive, computed, watch } from "vue";

const price = ref(0);
const vehicleType = ref("Common");

// basic states
const isLoading = ref(false);
const errorMessage = ref("");

// object holding the API results
const fees = reactive({
  price: 0,
  basicBuyerFee: 0,
  sellerSpecialFee: 0,
  associationFee: 0,
  storageFee: 0,
  total: 0,
  result: false,
  message: "",
});

// helper to quick formatter for currency
const formatCurrency = (value) => {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD"
  }).format(value ?? 0);
};


// show fees only if total actually has something
const hasResult = computed(() => fees.total > 0 && fees.result);

// helper to reset everything
const resetFees = () => {
  Object.assign(fees, {
    price: 0,
    basicBuyerFee: 0,
    sellerSpecialFee: 0,
    associationFee: 0,
    storageFee: 0,
    total: 0
  });
};

// helper to check if is a number some value (string or number variables)
const isNumber = (v) => Number.isFinite(Number(v));

// calling the API (nothing special)
const fetchCalculation = async () => {
  errorMessage.value = "";

  if (!price.value || price.value <= 0) {
    resetFees();
    if (isNumber(price.value)) {
      errorMessage.value = "Equal or less than zero price is not allowed.";
    }
    return;
  }

  isLoading.value = true;

  try {
    const response = await fetch("/api/bids/calculate", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        price: Number(price.value),
        vehicleType: vehicleType.value
      })
    });

    const data = await response.json().catch(() => ({}));

    if (!response.ok) {
      errorMessage.value = data?.message || "Error calling the API.";
      return;
    }

    if (!data.result) {
      errorMessage.value = data.message || "Calculation failed.";
      resetFees();
      return;
    }

    Object.assign(fees, data);

  } catch {
    errorMessage.value = "Couldn't reach the server.";
  } finally {
    isLoading.value = false;
  }
};


// light debounce so we don't hammer the API
let debounceHandle;

watch([price, vehicleType], () => {
  resetFees();
  clearTimeout(debounceHandle);
  debounceHandle = setTimeout(fetchCalculation, 250);
});
</script>





<style scoped>
.card { border:1px solid #ccc; padding:1rem; background:#fff; }

.card-header h2 { margin:0 0 .25rem; }
.card-header p { margin:0; font-size:.85rem; color:#555; }

.form-grid { margin-top:1rem; display:flex; gap:1rem; flex-wrap:wrap; }
.form-field { display:flex; flex-direction:column; min-width:170px; flex:1; }

label { font-size:.85rem; margin-bottom:.25rem; }

input, select {
  padding:.4rem;
  border:1px solid #bbb;
  font-size:.95rem;
}

.error-message { margin-top:.75rem; color:#a33; }
.hint { margin-top:.75rem; color:#555; }

.fees { margin-top:1.2rem; }
.fees ul { list-style:none; padding:0; margin:0; }
.fees li { display:flex; justify-content:space-between; padding:.3rem 0; border-bottom:1px solid #eee; }

.total {
  margin-top:.75rem;
  padding-top:.75rem;
  border-top:1px solid #ccc;
  display:flex;
  justify-content:space-between;
  font-weight:bold;
}
</style>
