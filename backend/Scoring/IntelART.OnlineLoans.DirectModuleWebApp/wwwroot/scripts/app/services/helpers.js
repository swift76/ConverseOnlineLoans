define(['knockout', 'constants/date'], function(ko, date) {
  'use strict';
  return {
    getParent: function getParent(target) {
      var context = ko.contextFor(target);
      return context ? context.$parent : {};
    },

    addHiddenDivToBody: function addHiddenDivToBody() {
      var div = document.createElement('div');
      div.style.display = 'none';
      document.body.appendChild(div);
      return div;
    },

    toUtcMidnight: function toUtcMidnight(date) {
      date.setHours(date.getHours() - date.getTimezoneOffset() / 60);
      return date.toISOString();
    },

    buildDaysArray: function buildDaysArray() {
      var days = [];
      for (var day = 1; day < 32; day++) {
        days.push({
          NAME: String(day),
          CODE: ('0' + day).slice(-2),
        });
      }
      return days;
    },

    buildMonthArray: function buildMonthArray() {
      var monthArray = [];
      var monthNames = date.monthNames;
      for (var month = 0; month < monthNames.length; month++) {
        monthArray.push({
          NAME: String(monthNames[month]),
          CODE: ('0' + (month + 1)).slice(-2),
        });
      }
      return monthArray;
    },

    buildYearArray: function buildYearArray() {
      var curentYear = new Date().getFullYear();
      var yearArray = [];
      for (var i = curentYear - 16; i >= curentYear - 70; i--) {
        yearArray.push({
          NAME: String(i),
          CODE: i,
        });
      }
      return yearArray;
    },

    formatMoney: function formatMoney(number) {
      return number
        .toFixed(2)
        .toString()
        .replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    },

    getMaxValueInArr: function(arr, key) {
      return arr.reduce(function(max, p) {
        return p[key] > max ? p[key] : max;
      }, arr[0][key]);
    },

    getMinValueInArr: function(arr, key) {
      return arr.reduce(function(max, p) {
        return p[key] < max ? p[key] : max;
      }, arr[0][key]);
    },

    calc: function calc(months, price, perc) {
      var list = [];
      list[0] = {
        price: price,
        x: 0,
        str1: 0,
        str2: 0,
        y: 0,
      };

      var interest = perc / 100;

      var result =
        (((interest / 12) * Math.pow(1 + interest / 12, months)) /
          (Math.pow(1 + interest / 12, months) - 1)) *
        price;

      var rou_result = Math.floor(result);

      var monthly_payment = rou_result + 0;
      var interest_paid = 0;
      var total_paid_amount = price + 0 + 0;
      var total_other = 0;

      for (var m = 1; m <= months; m++) {
        var current_percent = (list[m - 1].price * interest) / 12;
        var current_fading = result - current_percent;
        var current_capital = list[m - 1].price - current_fading;
        var rou_capital = Math.floor(current_capital);
        if (m === months) {
          rou_capital = 0;
        }
        var rou_percent = Math.floor(current_percent);
        var rou_fading = Math.floor(current_fading);
        list.push({
          price: rou_capital,
          x: 0,
          str1: rou_percent,
          str2: rou_fading,
          y: rou_result + 0,
        });
        interest_paid += rou_percent;

        total_paid_amount += 0 + rou_percent;

        total_other += 0;
      }
      return total_paid_amount / (m - 1);
    },
  };
});
