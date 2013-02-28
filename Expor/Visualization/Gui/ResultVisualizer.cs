using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results;

namespace Socona.Expor.Visualization.Gui
{

    public class ResultVisualizer : IResultHandler
    {
        ///**
        // * Get a logger for this class.
        // */
        //protected final static Logging logger = Logging.getLogger(ResultVisualizer.class);

        ///**
        // * Parameter to specify the window title
        // * <p>
        // * Key: {@code -vis.window.title}
        // * </p>
        // * <p>
        // * Default value: "ELKI Result Visualization"
        // * </p>
        // */
        //public static final OptionID WINDOW_TITLE_ID = OptionID.getOrCreateOptionID("vis.window.title", "Title to use for visualization window.");

        ///**
        // * Flag to set single display
        // * 
        // * <p>
        // * Key: -vis.single
        // * </p>
        // */
        //public final static OptionID SINGLE_ID = OptionID.getOrCreateOptionID("vis.window.single", "Embed visualizers in a single window, not using thumbnails and detail views.");

        ///**
        // * Stores the set title.
        // */
        //String title;

        ///**
        // * Default title
        // */
        //protected final static String DEFAULT_TITLE = "ELKI Result Visualization";

        ///**
        // * Visualization manager.
        // */
        //VisualizerParameterizer manager;

        ///**
        // * Single view mode
        // */
        //boolean single;

        ///**
        // * Constructor.
        // * 
        // * @param title Window title
        // * @param manager Parameterization manager for visualizers
        // * @param single Flag to indicat single-view mode.
        // */
        //public ResultVisualizer(String title, VisualizerParameterizer manager, boolean single) {
        //  super();
        //  this.title = title;
        //  this.manager = manager;
        //  this.single = single;
        //}

        //@Override
        //public void processNewResult(final HierarchicalResult top, final Result result) {
        //  // FIXME: not really re-entrant to generate new contexts...
        //  final VisualizerContext context = manager.newContext(top);

        //  if(title == null) {
        //    title = VisualizerParameterizer.getTitle(ResultUtil.findDatabase(top), result);
        //  }

        //  if(title == null) {
        //    title = DEFAULT_TITLE;
        //  }

        //  javax.swing.SwingUtilities.invokeLater(new Runnable() {
        //    @Override
        //    public void run() {
        //      try {
        //        ResultWindow window = new ResultWindow(title, top, context, single);
        //        window.setVisible(true);
        //        window.setExtendedState(window.getExtendedState() | JFrame.MAXIMIZED_BOTH);
        //        window.showOverview();
        //      }
        //      catch(Throwable e) {
        //        logger.exception("Error in starting visualizer window.", e);
        //      }
        //    }
        //  });
        //}

        ///**
        // * Parameterization class.
        // * 
        // * @author Erich Schubert
        // * 
        // * @apiviz.exclude
        // */
        //public static class Parameterizer extends AbstractParameterizer {
        //  /**
        //   * Stores the set title.
        //   */
        //  String title;

        //  /**
        //   * Visualization manager.
        //   */
        //  VisualizerParameterizer manager;

        //  /**
        //   * Single view mode.
        //   */
        //  boolean single = false;

        //  @Override
        //  protected void makeOptions(Parameterization config) {
        //    super.makeOptions(config);
        //    StringParameter titleP = new StringParameter(WINDOW_TITLE_ID, true);
        //    if(config.grab(titleP)) {
        //      title = titleP.getValue();
        //    }
        //    Flag singleF = new Flag(SINGLE_ID);
        //    if (config.grab(singleF)) {
        //      single = singleF.getValue();
        //    }
        //    manager = config.tryInstantiate(VisualizerParameterizer.class);
        //  }

        //  @Override
        //  protected ResultVisualizer makeInstance() {
        //    return new ResultVisualizer(title, manager, single);
        //  }
        //}
        public void ProcessNewResult(IHierarchicalResult baseResult, IResult newResult)
        {
            throw new NotImplementedException();
        }
    }
}
