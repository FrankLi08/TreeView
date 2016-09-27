(function ($) {
    function Tree() {
        var $this = this;
        function treeNodeClick() {
            $(document).on("click", '.tree li a input[type="checkbox"]', function () {
                $(this).closest("li").find('ul input[type="checkbox"]').prop("checked", $(this).is(":checked"));
                e.preventDefault();
            }).on("click", ".node-item", function () {
                var parentNode = $(this).parents(".tree ul");
                if ($(this).is(":checked")) {
                    parentNode.find("li a .parent").prop("checked", true);
                } else {
                    var elements = parentNode.find('ul input[type="checkbox"]:checked');
                    if (elements.length === 0) {
                        parentNode.find("li a .parent").prop("checked", false);
                    }
                }
            });
        };
        $this.init = function () {
            treeNodeClick();
        };
    }

    $(document).ready(function () {
        $("#DeleteSelected").on("click", function () {
            var id = [];
            $("input:checked").each(function () {
                id.push($(this).attr("value"));
            });

            $.ajax({
                url: "/Activities/DeleteSelected",
                type: "POST",
                data: { ids: id },
                traditional: true,
                success: function () {
                    window.location = "";
                }
            });
        });

        $("#AddRoot").on("click", function () {
            window.location = "/Activities/Add/0";
        });

        $("#GoBack").on("click", function () {
            window.location = "/Activities/Index";
        });
        $("#SortAsc").on("click", function () {
            window.location = "/Activities/Sort/0/asc";
        });
        $("#SortDesc").on("click", function () {
            window.location = "/Activities/Sort/0/desc";
        });
    });

    $(function () {
        var self = new Tree();
        self.init();
    });
}(jQuery))