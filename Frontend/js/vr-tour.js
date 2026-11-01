document.addEventListener("DOMContentLoaded", function () {
    const scene = document.querySelector("#vrScene");

    if (scene) {
        // إجبار A-Frame على إعادة حساب أبعاد الشاشة وإحداثيات الماوس
        const refreshVR = () => {
            if (scene.resize) {
                scene.resize();
            }
            window.dispatchEvent(new Event("resize"));
        };

        if (scene.hasLoaded) {
            refreshVR();
        } else {
            scene.addEventListener("loaded", refreshVR);
        }

        setTimeout(refreshVR, 200);
    }
});