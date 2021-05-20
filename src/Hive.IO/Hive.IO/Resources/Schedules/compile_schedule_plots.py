import matplotlib.pyplot as plt
import json
import os

def create_schedule_plot():
    """Creates simplified schedules to SVG based on SIA 2024"""
    filepath = os.path.abspath(os.path.join(os.path.abspath(__file__), '../../../Building/sia2024_schedules.json'))
    assert os.path.exists(filepath)
    
    save_dir = os.path.dirname(os.path.abspath(__file__))
    
    with open(filepath, 'r') as fp:
        schedules = json.load(fp)

    hours = list(range(0,24))
    multiplier = [0.0, 0.2, 0.4, 0.6, 0.8, 1.0]
    
    for s in schedules:
        plt.clf()
        
        fig, axs = plt.subplots(3, 1)
        fig.set_size_inches(361/50,268/50) # dims in the Winforms
        plt.setp(axs, xticks=hours, xticklabels=hours,
                 yticks=multiplier, yticklabels=multiplier,
                 xlim=[-1, 24], ylim=[0.0, 1.0])
        axs[0].bar(hours, s['OccupancySchedule']['DailyProfile'], color='black')
        axs[0].set_title('Occupancy')
        
        axs[1].bar(hours, s['DevicesSchedule']['DailyProfile'], color='black')
        axs[1].set_title('Devices')

        plt.xticks(ticks=hours, labels=hours)
        fig.tight_layout()
        plt.savefig(os.path.join(save_dir, f"{s['RoomType']}.png"))
        print(f"Saved {s['RoomType']}.png")
        # plt.show()


if __name__ == '__main__':
    create_schedule_plot()