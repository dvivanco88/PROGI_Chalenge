import { mount, flushPromises } from "@vue/test-utils";
import { describe, it, expect, vi } from "vitest";
import BidCalculatorView from "../src/components/BidCalculatorView.vue";

// Helper to wait debounce time
const waitDebounce = (ms = 300) =>
  new Promise(resolve => setTimeout(resolve, ms));

describe("BidCalculatorView", () => {
  it("renders input fields and select", () => {
    const wrapper = mount(BidCalculatorView);
    expect(wrapper.find("input#price").exists()).toBe(true);
    expect(wrapper.find("select#type").exists()).toBe(true);
  });

  it("formats currency values correctly", () => {
    const wrapper = mount(BidCalculatorView);
    const format = wrapper.vm.formatCurrency;

    expect(format(100)).toBe("$100.00");
    expect(format(1234.5)).toBe("$1,234.50");
  });

  it("updates the result after calling the backend", async () => {
    // Mock del backend
    global.fetch = vi.fn(() =>
      Promise.resolve({
        ok: true,
        json: () =>
          Promise.resolve({
            price: 1000,
            basicBuyerFee: 50,
            sellerSpecialFee: 20,
            associationFee: 10,
            storageFee: 100,
            total: 1180,
            result: true,
            message: "OK"
          })
      })
    );

    const wrapper = mount(BidCalculatorView);

    // Change input values
    await wrapper.find("input#price").setValue(1000);
    await wrapper.find("select#type").setValue("Common");

    // Wait for debounce + async fetch + Vue rendering
    await waitDebounce(300);
    await flushPromises();
    await flushPromises();

    expect(wrapper.html()).toContain("$1,180.00");
  });
});
