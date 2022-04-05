
// Multi Level Dropdown
/*
function dropDown(a) {
  if (!document.querySelector('.dropdown-hover')) {
    event.stopPropagation();
    event.preventDefault();
    var multilevel = a.parentElement.parentElement.children;

    for (var i = 0; i < multilevel.length; i++) {
      if (multilevel[i].lastElementChild != a.parentElement.lastElementChild) {
        multilevel[i].lastElementChild.classList.remove('show');
      }
    }

    if (!a.nextElementSibling.classList.contains("show")) {
      a.nextElementSibling.classList.add("show");
    } else {
      a.nextElementSibling.classList.remove("show");
    }
  }
}
*/