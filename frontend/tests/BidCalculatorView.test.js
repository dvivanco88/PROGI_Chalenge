import { mount, flushPromises } from "@vue/test-utils";
import { describe, it, expect, vi } from "vitest";
import BidCalculatorView from "../src/components/BidCalculatorView.vue";

// Helper to wait the debounce delay used inside the component
const waitDebounce = (ms = 300) =>
  new Promise(resolve => setTimeout(resolve, ms));

describe("BidCalculatorView - PDF Specification Test Cases", () => {
  
  const testCases = [
    // price, type, basic, special, association, storage, total
    [398.00, "Common", 39.80, 7.96, 5.00, 100.00, 550.76],
    [501.00, "Common", 50.00, 10.02, 10.00, 100.00, 671.02],
    [57.00,  "Common", 10.00, 1.14, 5.00, 100.00, 173.14],
    [1800.00, "Luxury", 180.00, 72.00, 15.00, 100.00, 2167.00],
    [1100.00, "Common", 50.00, 22.00, 15.00, 100.00, 1287.00],
    [1000000.00, "Luxury", 200.00, 40000.00, 20.00, 100.00, 1040320.00]
  ];

  // Generate one test per PDF case
  testCases.forEach(([price, type, basic, special, assoc, storage, total]) => {
    it(`matches PDF case -> price: ${price}, type: ${type}`, async () => {

      // Mock backend
      global.fetch = vi.fn(() =>
        Promise.resolve({
          ok: true,
          json: () =>
            Promise.resolve({
              price,
              basicBuyerFee: basic,
              sellerSpecialFee: special,
              associationFee: assoc,
              storageFee: storage,
              total,
              result: true,
              message: "OK"
            })
        })
      );

      const wrapper = mount(BidCalculatorView);

      await wrapper.find("input#price").setValue(price);
      await wrapper.find("select#type").setValue(type);

      await waitDebounce(300);
      await flushPromises();
      await flushPromises();

      const formattedTotal = wrapper.vm.formatCurrency(total);
      expect(wrapper.html()).toContain(formattedTotal);
    });
  });

});
