window.leafletFunctions = {

    // Safe availability check — called instead of eval()
    isAvailable: function () {
        return typeof L !== 'undefined' && typeof L.map === 'function';
    },

    initMap: function (elementId, centerLat, centerLng, zoom) {
        try {
            // Clear any existing map
            if (window._complaintsMap) {
                window._complaintsMap.remove();
                window._complaintsMap = null;
            }

            // Ensure the element exists
            const element = document.getElementById(elementId);
            if (!element) {
                console.error('Map container element not found:', elementId);
                return false;
            }

            // Ensure element has a concrete height before Leaflet measures it
            if (!element.offsetHeight) {
                element.style.height = '600px';
            }

            // Create the map
            const map = L.map(elementId, {
                center: [centerLat, centerLng],
                zoom: zoom,
                zoomControl: true,
                attributionControl: true
            });

            // Add tile layer
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
                maxZoom: 19
            }).addTo(map);

            // Store map instance
            window._complaintsMap = map;

            // Force Leaflet to recalculate size after Blazor finishes rendering
            setTimeout(function () {
                if (window._complaintsMap) {
                    window._complaintsMap.invalidateSize();
                }
            }, 200);

            console.log('Map initialized successfully');
            return true;
        } catch (error) {
            console.error('Error initializing map:', error);
            return false;
        }
    },

    addMarker: function (lat, lng, popupHtml) {
        try {
            if (!window._complaintsMap) {
                console.error('Map not initialized');
                return false;
            }

            const marker = L.marker([lat, lng]).addTo(window._complaintsMap);
            if (popupHtml) {
                marker.bindPopup(popupHtml, { maxWidth: 260 });
            }

            return true;
        } catch (error) {
            console.error('Error adding marker:', error);
            return false;
        }
    },

    setView: function (lat, lng, zoom) {
        try {
            if (!window._complaintsMap) {
                console.error('Map not initialized');
                return false;
            }

            window._complaintsMap.setView([lat, lng], zoom);
            return true;
        } catch (error) {
            console.error('Error setting map view:', error);
            return false;
        }
    },

    clearMarkers: function () {
        try {
            if (!window._complaintsMap) {
                return false;
            }

            window._complaintsMap.eachLayer(function (layer) {
                if (layer instanceof L.Marker) {
                    window._complaintsMap.removeLayer(layer);
                }
            });

            return true;
        } catch (error) {
            console.error('Error clearing markers:', error);
            return false;
        }
    }
};
