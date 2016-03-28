function calculateCost(topIngredients, insideIngredients, options, qty) {
    /* Check for any ingredients selected, otherwise return null */
    if (topIngredients.length < 1 && insideIngredients.length < 1 && options.length <1) { return null; }

    /* Set base roll cost of all rolls */
    var rollCost = 2.00;
    var toppingCount = 0;

    for (i = 0; i < topIngredients.length; i++) {
        if (topIngredients[i].Group == "Toppings") { toppingCount++; }
    }

    /* Get prices for all top ingredients selected */
    for (i = 0; i < topIngredients.length; i++) {
        /* Check if ingredient affects the number of ingredients used - e.g. is a sauce or not */
        if(topIngredients.length - toppingCount == 1 
            && topIngredients[i].Group != "Toppings"){
                rollCost += topIngredients[i].Price * 2; /* Only 1 non-sauce ingredient, more ingredients required so double price */
        } else{rollCost += topIngredients[i].Price;} /* Multiple ingredients so use regular price */
    }
    /* Get prices for inside ingredients selected */
    for (i = 0; i < insideIngredients.length; i++){ rollCost += insideIngredients[i].Price; }
    /* Get prices for options selected */
    for (i = 0; i < options.length; i++) { rollCost += options[i].Price; }

        if (rollCost == 2.00) {
            return "";
        } else {
            /* Return total cost divided by 100 (to create cents) */
            return ((rollCost) * qty).toFixed(2);
        }
}

function removeArrayItem(array, value) {
    /* Find unchecked ingredient in the array */
    for(i = 0; i < array.length; i++){
        if (array[i].Id == value){ array.splice(i,1); } /* Remove ingredient from the array */
    }

    /* Return the new array */
    return array;
}

function lookup(array, prop, value){
    for (var i = 0, len = array.length; i < len; i++){
        if (array[i][prop] == value) return array[i];
    }
}